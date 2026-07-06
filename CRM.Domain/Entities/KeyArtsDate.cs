using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// A recurring arts date, heritage month, or observance to plan around. Annual reference
/// data grouped by month. <see cref="DateText"/> is free text because the source dates are
/// often relative or spanning ("3rd Mon", "Full month", "9/15-10/15", "Varies").
/// </summary>
public class KeyArtsDate : BaseEntity
{
    public int Month { get; set; }
    public int SortOrder { get; set; }
    public string DateText { get; set; } = string.Empty;
    public string Observance { get; set; } = string.Empty;
    public string? ObservanceType { get; set; }
    public string? ProgrammingTieIn { get; set; }
}
