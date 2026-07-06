using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

/// <summary>
/// One month of the Annual Programming Calendar — its theme, the season's programming, and
/// its production arc. An annual template keyed by <see cref="Month"/> (1–12, unique): the
/// calendar is a living yearly plan that evolves in place rather than per-year records.
/// </summary>
public class CalendarTheme : BaseEntity
{
    public int Month { get; set; }
    public string ThemeTitle { get; set; } = string.Empty;
    public string? ThemeSubtitle { get; set; }
    public string? KeyArtsDatesText { get; set; }
    public string? FeaturedGamesText { get; set; }
    public string? AlternativeOptionsText { get; set; }
    public string? ProductionPhase { get; set; }
    public string? ProgrammingNotes { get; set; }
    public ThemeArc? LegendArc { get; set; }
}
