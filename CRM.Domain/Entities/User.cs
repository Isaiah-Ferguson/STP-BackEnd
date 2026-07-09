using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    /// <summary>Base64-encoded PBKDF2 hash of the password + salt.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Base64-encoded per-user random salt.</summary>
    public string PasswordSalt { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Staff;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    /// <summary>Optimistic-concurrency token (#26) — concurrent full-row updates now
    /// surface as conflicts instead of silently overwriting each other.</summary>
    public byte[] RowVersion { get; set; } = [];

    /// <summary>Optional link to a staff record. Admins need not be staff.</summary>
    public Guid? StaffMemberId { get; set; }
    public StaffMember? StaffMember { get; set; }
}
