using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// A Section-6 note a teacher recorded for a Star in one week: a Strength, Area, or New
/// Goal, chosen from the <see cref="GoalBankEntry"/> bank or typed free-hand. One per
/// (participant, month, week, kind).
/// </summary>
public class WeeklyNoteSelection : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public GoalBankKind Kind { get; set; }

    public Guid? GoalBankEntryId { get; set; }
    public string? CustomText { get; set; }

    public Participant? Participant { get; set; }
    public GoalBankEntry? GoalBankEntry { get; set; }
}
