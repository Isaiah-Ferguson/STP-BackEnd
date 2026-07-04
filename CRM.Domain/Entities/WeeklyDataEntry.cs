using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// One weekly Data score for a Star on a single sub-skill. Recorded only for the week's
/// focus skills, only for Stars present. <see cref="SessionId"/> bridges into the
/// session/attendance graph (scoping + date); <see cref="MonthKey"/> ("yyyy-MM") and
/// <see cref="WeekNumber"/> anchor it in the month. One entry per
/// (participant, sub-skill, month, week) — enforced in the service (upsert).
/// </summary>
public class WeeklyDataEntry : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid SubSkillId { get; set; }
    public Guid? SessionId { get; set; }

    public string MonthKey { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public DateTime WeekDate { get; set; }

    public DataScore Score { get; set; }
    public Guid? RecordedByStaffMemberId { get; set; }

    public Participant? Participant { get; set; }
    public SubSkill? SubSkill { get; set; }
}
