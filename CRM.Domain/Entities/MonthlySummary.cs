using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// The end-of-month wrap for a Star: their primary level for the month, a short progress
/// note, whether goals carry over, and (if not) what's new next month. One per
/// (participant, month).
/// </summary>
public class MonthlySummary : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public string MonthKey { get; set; } = string.Empty;

    public ProgressLevel PrimaryLevel { get; set; }
    public string? ProgressNarrative { get; set; }
    public bool GoalsCarryOver { get; set; } = true;
    public string? NextMonthUpdate { get; set; }

    public Participant? Participant { get; set; }
}
