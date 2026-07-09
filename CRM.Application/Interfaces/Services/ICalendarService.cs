using CRM.Application.DTOs.Calendar;

namespace CRM.Application.Interfaces.Services;

public interface ICalendarService
{
    Task<IReadOnlyList<CalendarEventDto>> GetEventsAsync(int month, int year, CancellationToken ct = default);
    Task<CalendarEventDto> CreateEventAsync(CreateCalendarEventDto dto);
}
