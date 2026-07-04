using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// A tunable cutoff for month-end derivation: the minimum average weekly Data score
/// (0–3) at which a skill reaches a given Progress Level. Seeded (Novice 0.0,
/// Intermediate 1.5, Expert 2.5) but editable without code, so cutoffs can be adjusted.
/// </summary>
public class ScoreThreshold : BaseEntity
{
    public ProgressLevel Level { get; set; }
    public double MinAverage { get; set; }
}
