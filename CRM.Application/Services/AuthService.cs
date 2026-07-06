using CRM.Application.DTOs.Auth;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(IUnitOfWork uow, IPasswordHasher hasher, ITokenService tokens)
    {
        _uow = uow;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<AuthResultDto?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _uow.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Run the hash check even when the user is missing to avoid leaking
        // account existence via response timing.
        var hash = user?.PasswordHash ?? string.Empty;
        var salt = user?.PasswordSalt ?? string.Empty;
        var valid = _hasher.VerifyPassword(dto.Password, hash, salt);

        if (user is null || !user.IsActive || !valid)
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();

        // Include the linked staff role so management-write policies can allow Coordinators.
        StaffRole? staffRole = user.StaffMemberId is { } sid
            ? (await _uow.Staff.GetByIdAsync(sid))?.Role
            : null;

        var (token, expiresAt) = _tokens.CreateToken(user, staffRole);
        return new AuthResultDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = ToDto(user),
        };
    }

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

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
            throw new InvalidOperationException($"Password must be at least {MinPasswordLength} characters.");
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
