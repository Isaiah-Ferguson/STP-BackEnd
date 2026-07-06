using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// A canned Section-6 example — a Strength, Area for Improvement, or New Goal — tagged by
/// skill Section (1–5) and level, mirroring the tracker's example bank. New Goals carry a
/// built-in "+1 growing edge". Seeded from the spreadsheet; teachers pick these in the
/// weekly notes (or type their own). <see cref="SubSkillId"/> is optional for future finer
/// tagging; seeded entries are keyed by Section + level only.
/// </summary>
public class GoalBankEntry : BaseEntity
{
    public GoalBankKind Kind { get; set; }
    public int SectionNumber { get; set; }
    public ProgressLevel Level { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool HasGrowingEdge { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? SubSkillId { get; set; }
}
