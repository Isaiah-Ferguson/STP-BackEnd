using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// A long-lived, rotating credential that lets a client mint a new short-lived JWT
/// without re-entering a password (#17/#20). Only a SHA-256 hash of the token is stored —
/// a database leak must not yield usable credentials. Rotated on every use: the old row
/// is revoked and a replacement issued, so a stolen-and-replayed token is detectable.
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    /// <summary>Base64 SHA-256 of the raw token handed to the client.</summary>
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
