using CRM.Application.DTOs.Calendar;

namespace CRM.Application.Interfaces.Services;

public interface IYearCalendarService
{
    /// <summary>The full annual calendar — 12 monthly themes + recurring key arts dates.</summary>
    Task<YearCalendarDto> GetCalendarAsync();

    /// <summary>Creates or updates the theme for a month (1–12).</summary>
    Task<CalendarThemeDto> UpsertThemeAsync(UpsertCalendarThemeDto dto);

    Task<IReadOnlyList<KeyArtsDateDto>> GetKeyArtsDatesAsync();
}
