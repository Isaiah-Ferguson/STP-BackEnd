using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// A named ability/cohort group a Star belongs to (Bright Stars, Glowing Stars,
/// Sparkling Stars). Named <c>StarGroup</c> to avoid colliding with the free-text
/// <see cref="AttendanceRecord"/> group field. Reference data, seeded via migration.
/// </summary>
public class StarGroup : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
