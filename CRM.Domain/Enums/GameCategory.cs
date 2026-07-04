namespace CRM.Domain.Enums;

/// <summary>
/// The broad grouping a game belongs to, from the Games Library section headers.
/// Drives the category tabs in the browser. The finer free-text label (e.g.
/// "Vocal Warmup", "Grounding / Theater") is kept separately on <c>Game.CategoryLabel</c>.
/// </summary>
public enum GameCategory
{
    Warmup,
    Circle,
    Movement,
    Name,
    Icebreaker,
    Theater,
    Reset,
    SuggestedAddition,
}
