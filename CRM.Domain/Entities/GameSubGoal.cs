using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// Tags a <see cref="Game"/> to a <see cref="SubSkill"/> it develops. One row per
/// sub-goal, with the first (lowest <see cref="SortOrder"/>) flagged
/// <see cref="IsPrimary"/>. A surrogate-key join (rather than composite) so it fits
/// the generic-repository pattern used across the domain.
/// </summary>
public class GameSubGoal : BaseEntity
{
    public Guid GameId { get; set; }
    public Guid SubSkillId { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }

    public Game? Game { get; set; }
    public SubSkill? SubSkill { get; set; }
}
