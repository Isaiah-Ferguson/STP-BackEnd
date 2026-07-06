using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// A game idea in the "To Develop" backlog — captured, refined, then promoted into the
/// Games Library. <see cref="TeacherSuggested"/> flags a staff-suggested idea even when no
/// specific member is attributed via <see cref="TeacherSuggestedId"/>. Once promoted,
/// <see cref="PromotedGameId"/> links to the created <see cref="Game"/>.
/// </summary>
public class GameIdea : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? StatusNotes { get; set; }
    public string? SourceInspiration { get; set; }
    public GameCategory? TargetCategory { get; set; }

    public bool TeacherSuggested { get; set; }
    public Guid? TeacherSuggestedId { get; set; }

    public Guid? PromotedGameId { get; set; }
}
