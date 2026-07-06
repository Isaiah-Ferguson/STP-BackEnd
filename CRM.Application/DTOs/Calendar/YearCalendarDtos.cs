using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Calendar;

public class CalendarThemeDto
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

public class UpsertCalendarThemeDto
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

public class KeyArtsDateDto
{
    public Guid Id { get; set; }
    public int Month { get; set; }
    public int SortOrder { get; set; }
    public string DateText { get; set; } = string.Empty;
    public string Observance { get; set; } = string.Empty;
    public string? ObservanceType { get; set; }
    public string? ProgrammingTieIn { get; set; }
}

/// <summary>The whole annual calendar: the 12 monthly themes plus the recurring key arts dates.</summary>
public class YearCalendarDto
{
    public List<CalendarThemeDto> Themes { get; set; } = new();
    public List<KeyArtsDateDto> KeyArtsDates { get; set; } = new();
}
