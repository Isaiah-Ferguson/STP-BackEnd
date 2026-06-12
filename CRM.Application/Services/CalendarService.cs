using CRM.Application.DTOs.Calendar;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class CalendarService : ICalendarService
{
    private readonly IUnitOfWork _uow;

    public CalendarService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<CalendarEventDto>> GetEventsAsync(int month, int year)
    {
        var events = await _uow.CalendarEvents.GetAllAsync();
        var programs = await _uow.Programs.GetAllAsync();
        var programMap = programs.ToDictionary(p => p.Id, p => p.Name);

        return events
            .Where(e => e.Date.Month == month && e.Date.Year == year)
            .OrderBy(e => e.Date)
            .Select(e => ToDto(e, programMap))
            .ToList();
    }

    public async Task<CalendarEventDto> CreateEventAsync(CreateCalendarEventDto dto)
    {
        var date = DateTime.Parse(dto.Date, null, System.Globalization.DateTimeStyles.RoundtripKind);

        var ev = new CalendarEvent
        {
            Title = dto.Title,
            Date = date,
            ProgramId = dto.ProgramId,
            Location = dto.Location,
            Meta = dto.Meta,
            TimeRange = dto.TimeRange,
            IsUpcoming = date >= DateTime.UtcNow,
        };

        await _uow.CalendarEvents.AddAsync(ev);
        await _uow.SaveChangesAsync();

        var programs = await _uow.Programs.GetAllAsync();
        var programMap = programs.ToDictionary(p => p.Id, p => p.Name);

        return ToDto(ev, programMap);
    }

    private static CalendarEventDto ToDto(CalendarEvent e, Dictionary<Guid, string> programMap) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Location = e.Location,
        Meta = e.Meta,
        Date = e.Date.ToString("yyyy-MM-dd"),
        TimeRange = e.TimeRange,
        ProgramId = e.ProgramId,
        ProgramName = e.ProgramId.HasValue ? programMap.GetValueOrDefault(e.ProgramId.Value) : null,
        IsUpcoming = e.IsUpcoming,
    };
}
