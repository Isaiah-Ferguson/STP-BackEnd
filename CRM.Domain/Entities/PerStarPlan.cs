using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// A staff member's monthly plan for one Star (the "Per-Star Planning" tab): their primary
/// tier, the priority objective area + sub-skill to work on, a monthly goal with a +1
/// growing edge, and how the teacher will support it in the group project. Month-scoped
/// (MonthKey "yyyy-MM") so each month is its own snapshot; one per (participant, month).
/// </summary>
public class PerStarPlan : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public string MonthKey { get; set; } = string.Empty;

    public ProgressLevel PrimaryTier { get; set; }
    public Guid? PriorityObjectiveAreaId { get; set; }
    public Guid? PrioritySubSkillId { get; set; }

    public string? MonthlyGoal { get; set; }
    public string? HowIllSupport { get; set; }
    public string? Notes { get; set; }

    public Participant? Participant { get; set; }
    public StaffMember? AssignedStaff { get; set; }
    public ObjectiveArea? PriorityObjectiveArea { get; set; }
    public SubSkill? PrioritySubSkill { get; set; }
}
