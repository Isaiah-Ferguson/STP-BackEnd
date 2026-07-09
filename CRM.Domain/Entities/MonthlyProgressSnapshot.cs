using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// A Star's month-end Progress Level on one sub-skill. <see cref="SuggestedLevel"/> is
/// derived from the weeks the skill was scored (average → threshold); <see cref="Level"/>
/// is the authoritative value a teacher confirms (they may override the suggestion). Only
/// snapshots with <see cref="IsConfirmed"/> feed the cohort roll-up. One per
/// (participant, sub-skill, month).
/// </summary>
public class MonthlyProgressSnapshot : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid SubSkillId { get; set; }
    public string MonthKey { get; set; } = string.Empty;

    public ProgressLevel Level { get; set; }
    public ProgressLevel SuggestedLevel { get; set; }

    /// <summary>Sum of the scored (non-N/A) weeks — informational.</summary>
    public int SummedScore { get; set; }
    /// <summary>How many weeks the skill was actually scored — the derivation denominator.</summary>
    public int ScoredWeekCount { get; set; }

    public bool IsConfirmed { get; set; }
    public Guid? ConfirmedByStaffMemberId { get; set; }

    /// <summary>Optimistic-concurrency token (#26) — concurrent full-row updates now
    /// surface as conflicts instead of silently overwriting each other.</summary>
    public byte[] RowVersion { get; set; } = [];

    public Participant? Participant { get; set; }
    public SubSkill? SubSkill { get; set; }
}
