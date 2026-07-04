using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// A game or exercise from the Games Library, tagged against the shared taxonomy so
/// staff can find activities by tier + objective area + sub-goal when planning. The
/// primary objective area is a single <see cref="ObjectiveArea"/>; the sub-goals (one
/// primary, then secondaries) are the <see cref="GameSubGoal"/> join onto sub-skills.
/// </summary>
public class Game : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public GameSource Source { get; set; }
    public GameCategory Category { get; set; }

    /// <summary>The finer free-text category from the spreadsheet (e.g. "Vocal Warmup").</summary>
    public string? CategoryLabel { get; set; }

    public GameTier Tiers { get; set; }

    public Guid PrimaryObjectiveAreaId { get; set; }

    public string? Description { get; set; }
    public string? BestForVariations { get; set; }
    public string? WhenToUse { get; set; }

    public ObjectiveArea? PrimaryObjectiveArea { get; set; }
    public ICollection<GameSubGoal> SubGoals { get; set; } = new List<GameSubGoal>();
}
