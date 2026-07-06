using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// A running-log entry: how a teacher modified a game for a particular age/group level.
/// Optionally links to the library <see cref="Game"/> it modifies.
/// </summary>
public class AgeModification : BaseEntity
{
    public string GameName { get; set; } = string.Empty;
    public string GroupAgeLevel { get; set; } = string.Empty;
    public string Modification { get; set; } = string.Empty;

    public bool TeacherSuggested { get; set; }
    public Guid? TeacherSuggestedId { get; set; }
    public Guid? GameId { get; set; }
}
