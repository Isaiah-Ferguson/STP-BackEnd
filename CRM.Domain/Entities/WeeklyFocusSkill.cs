using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// A sub-skill designated as a focus for a program in a given week (the 2–4 skills the
/// lesson plan targets). Drives which skills the weekly-entry grid shows, and defines the
/// denominator for month-end derivation. Scoped per program so each site's plan differs.
/// </summary>
public class WeeklyFocusSkill : BaseEntity
{
    public Guid ProgramId { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public Guid SubSkillId { get; set; }

    public CrmProgram? Program { get; set; }
    public SubSkill? SubSkill { get; set; }
}
