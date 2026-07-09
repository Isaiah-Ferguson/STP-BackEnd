using CRM.Application.DTOs.Auth;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork uow, IPasswordHasher hasher, ITokenService tokens, ILogger<AuthService> logger)
    {
        _uow = uow;
        _hasher = hasher;
        _tokens = tokens;
        _logger = logger;
    }

    public async Task<AuthSessionDto?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _uow.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Run the hash check even when the user is missing to avoid leaking
        // account existence via response timing.
        var hash = user?.PasswordHash ?? string.Empty;
        var salt = user?.PasswordSalt ?? string.Empty;
        var valid = _hasher.VerifyPassword(dto.Password, hash, salt);

        if (user is null || !user.IsActive || !valid)
        {
            // Log failed attempts so credential-stuffing shows up in the logs. Do not log
            // the password; the email is fine (it was submitted in cleartext anyway).
            var reason = user is null ? "no such user"
                : !user.IsActive ? "account inactive"
                : "bad password";
            _logger.LogWarning("Failed login for {Email}: {Reason}.", email, reason);
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _uow.Users.UpdateAsync(user);

        var session = await CreateSessionAsync(user);
        _logger.LogInformation("User {UserId} logged in.", user.Id);
        return session;
    }

    public async Task<AuthSessionDto?> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return null;

        var hash = HashRefreshToken(refreshToken);
        var stored = await _uow.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
        if (stored is null || !stored.IsActive)
        {
            _logger.LogWarning("Refresh rejected: token unknown, expired, or already used.");
            return null;
        }

        var user = await _uow.Users.GetByIdAsync(stored.UserId);
        if (user is null || !user.IsActive)
        {
            _logger.LogWarning("Refresh rejected for user {UserId}: account inactive.", stored.UserId);
            return null;
        }

        // Rotation: this token is single-use. Revoking before reissue means a replayed
        // (stolen) token fails loudly instead of silently minting sessions.
        stored.RevokedAt = DateTime.UtcNow;
        await _uow.RefreshTokens.UpdateAsync(stored);

        return await CreateSessionAsync(user);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return;

        var hash = HashRefreshToken(refreshToken);
        var stored = await _uow.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
        if (stored is null || stored.RevokedAt is not null) return;

        stored.RevokedAt = DateTime.UtcNow;
        await _uow.RefreshTokens.UpdateAsync(stored);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("User {UserId} logged out.", stored.UserId);
    }

    private const int RefreshTokenDays = 14;

    /// <summary>Mints a JWT + fresh refresh token for the user and persists pending changes.</summary>
    private async Task<AuthSessionDto> CreateSessionAsync(User user)
    {
        // Include the linked staff role so management-write policies can allow Coordinators.
        StaffRole? staffRole = user.StaffMemberId is { } sid
            ? (await _uow.Staff.GetByIdAsync(sid))?.Role
            : null;

        var (token, expiresAt) = _tokens.CreateToken(user, staffRole);

        var raw = GenerateRefreshToken();
        var refresh = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashRefreshToken(raw),
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenDays),
        };
        await _uow.RefreshTokens.AddAsync(refresh);
        await _uow.SaveChangesAsync();

        return new AuthSessionDto
        {
            Auth = new AuthResultDto { Token = token, ExpiresAt = expiresAt, User = ToDto(user) },
            RefreshToken = raw,
            RefreshExpiresAt = refresh.ExpiresAt,
        };
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48));

    private static string HashRefreshToken(string raw) =>
        Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw)));

    private const int MinPasswordLength = 8;

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        ValidatePassword(dto.Password);

        var existing = await _uow.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existing is not null)
            throw new InvalidOperationException($"A user with email '{email}' already exists.");

        var (hash, salt) = _hasher.HashPassword(dto.Password);

        var user = new User
        {
            Email = email,
            FullName = dto.FullName.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = dto.Role,
            StaffMemberId = dto.StaffMemberId,
            IsActive = true,
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _uow.Users.GetByIdAsync(id);
        return user is null ? null : ToDto(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        var users = await _uow.Users.GetAllAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, Guid actingUserId)
    {
        var user = await _uow.Users.GetByIdAsync(id);
        if (user is null) return null;

        var resultingRole = dto.Role ?? user.Role;
        var resultingActive = dto.IsActive ?? user.IsActive;

        // Block changes that would strip the system of its last usable admin.
        var losingAdmin = user.Role == UserRole.Admin && user.IsActive
                          && (resultingRole != UserRole.Admin || !resultingActive);
        if (losingAdmin)
            await GuardLastAdminAsync(id, actingUserId, "remove admin access from");

        if (dto.FullName is not null) user.FullName = dto.FullName.Trim();
        if (dto.Role.HasValue) user.Role = dto.Role.Value;
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
        if (dto.StaffMemberId.HasValue) user.StaffMemberId = dto.StaffMemberId;

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task<bool> ResetPasswordAsync(Guid id, ResetPasswordDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(id);
        if (user is null) return false;

        ValidatePassword(dto.NewPassword);

        var (hash, salt) = _hasher.HashPassword(dto.NewPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user is null) return false;

        if (!_hasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            throw new InvalidOperationException("Current password is incorrect.");

        ValidatePassword(dto.NewPassword);

        var (hash, salt) = _hasher.HashPassword(dto.NewPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid id, Guid actingUserId)
    {
        var user = await _uow.Users.GetByIdAsync(id);
        if (user is null) return false;

        if (user.Role == UserRole.Admin && user.IsActive)
            await GuardLastAdminAsync(id, actingUserId, "delete");

        await _uow.Users.DeleteAsync(user);
        await _uow.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Prevents an admin from locking everyone out: you cannot demote/disable/delete
    /// your own account, nor the final remaining active admin.
    /// </summary>
    private async Task GuardLastAdminAsync(Guid targetId, Guid actingUserId, string verb)
    {
        if (targetId == actingUserId)
            throw new InvalidOperationException($"You cannot {verb} your own account.");

        var activeAdmins = await _uow.Users.ListAsync(u => u.Role == UserRole.Admin && u.IsActive);
        if (activeAdmins.Count <= 1)
            throw new InvalidOperationException($"Cannot {verb} the last active admin.");
    }

    // Known-compromised defaults that must never be (re)set: the original seeder password
    // was committed to a public repo, so it is permanently burned (#21).
    private static readonly string[] BlockedPasswords = ["ChangeMe!123"];

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
            throw new InvalidOperationException($"Password must be at least {MinPasswordLength} characters.");

        if (BlockedPasswords.Any(p => string.Equals(p, password, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("That password is a known default and cannot be used. Choose a different one.");

        var hasLetter = password.Any(char.IsLetter);
        var hasDigit = password.Any(char.IsDigit);
        if (!hasLetter || !hasDigit)
            throw new InvalidOperationException("Password must contain at least one letter and one number.");
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        FullName = u.FullName,
        Role = u.Role,
        IsActive = u.IsActive,
        StaffMemberId = u.StaffMemberId,
    };
}
