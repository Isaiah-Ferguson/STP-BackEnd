using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// The "Student Frame" from the weekly tracker — the three personalised, management-authored
/// narrative fields that anchor the universal goal bank to an individual Star. A 1:1 satellite
/// on <see cref="Participant"/> (current-state, not month-scoped history). All three are free-text:
/// the per-skill Novice/Intermediate/Expert levels live in the monthly tracker, not here.
/// </summary>
public class ParticipantArtsProfile : BaseEntity
{
    public Guid ParticipantId { get; set; }

    /// <summary>Student IPP Summary.</summary>
    public string? IppSummary { get; set; }

    /// <summary>Student Current Level — a narrative baseline of present functioning.</summary>
    public string? CurrentLevel { get; set; }

    /// <summary>Student TSSP Arts Goal.</summary>
    public string? TsspArtsGoal { get; set; }

    public Participant? Participant { get; set; }
}
