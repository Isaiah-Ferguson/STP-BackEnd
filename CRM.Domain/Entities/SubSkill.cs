using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// A single universal skill measured on every Part-Time Star, nested under an
/// <see cref="ObjectiveArea"/>. The 18 sub-skills map onto the tracker's five
/// measured Sections (Section 1–5); the sixth objective area (Multi-Area) carries
/// no sub-skills and is a Games Library tag only. Reference data, seeded once via
/// migration. <see cref="IsActive"/> lets a skill be retired without deleting the
/// historical data that references it.
/// </summary>
public class SubSkill : BaseEntity
{
    public Guid ObjectiveAreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SectionNumber { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ObjectiveArea? ObjectiveArea { get; set; }
}
