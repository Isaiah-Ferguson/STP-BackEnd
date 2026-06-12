using CRM.Application.DTOs.Calendar;
using CRM.Application.DTOs.Participants;
using CRM.Application.DTOs.Programs;
using CRM.Application.DTOs.Staff;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class ProgramService : IProgramService
{
    private readonly IUnitOfWork _uow;

    public ProgramService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ProgramSummaryDto>> GetAllAsync()
    {
        var programs = await _uow.Programs.GetAllAsync();
        var participants = await _uow.Participants.GetAllAsync();
        var sessions = await _uow.Sessions.GetAllAsync();

        var ptsByProgram = participants
            .GroupBy(p => p.ProgramId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var nextByProgram = sessions
            .Where(s => s.Date >= DateTime.UtcNow)
            .GroupBy(s => s.ProgramId)
            .ToDictionary(g => g.Key, g => g.MinBy(s => s.Date));

        return programs.Select(p =>
        {
            var pts = ptsByProgram.GetValueOrDefault(p.Id, new());
            var next = nextByProgram.GetValueOrDefault(p.Id);
            return new ProgramSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ColorHex = p.ColorHex,
                SessionSchedule = p.SessionSchedule,
                DefaultLocation = p.DefaultLocation,
                EnrolledCount = pts.Count(x => x.Status == Domain.Enums.ParticipantStatus.Active),
                AttendancePct = pts.Any() ? (int)pts.Average(x => x.AttendancePct) : null,
                NextSessionDate = next?.Date.ToString("MMM d"),
                NextSessionMeta = next?.Room is string r ? $"{next.Date:dddd} · {r}" : null,
                AlertCount = pts.Count(x => x.Status == Domain.Enums.ParticipantStatus.Attention),
            };
        }).ToList();
    }

    public async Task<ProgramSummaryDto?> GetBySlugAsync(string slug)
    {
        var all = await GetAllAsync();
        return all.FirstOrDefault(p => p.Slug == slug);
    }

    public async Task<ProgramDetailDto?> GetDetailAsync(string slug)
    {
        var programs = await _uow.Programs.GetAllAsync();
        var program = programs.FirstOrDefault(p => p.Slug == slug);
        if (program is null) return null;

        var participants = (await _uow.Participants.GetAllAsync())
            .Where(p => p.ProgramId == program.Id).ToList();

        var sessions = (await _uow.Sessions.GetAllAsync())
            .Where(s => s.ProgramId == program.Id).ToList();

        var events = (await _uow.CalendarEvents.GetAllAsync())
            .Where(e => e.ProgramId == program.Id && e.IsUpcoming)
            .OrderBy(e => e.Date)
            .Take(5)
            .ToList();

        var allStaff = await _uow.Staff.GetAllAsync();
        var allAssignments = (await _uow.Programs.GetAllAsync()); // nav not loaded; use UoW.Staff approach below

        var summary = await GetBySlugAsync(slug);

        return new ProgramDetailDto
        {
            Id = program.Id,
            Name = program.Name,
            Slug = program.Slug,
            ColorHex = program.ColorHex,
            SessionSchedule = program.SessionSchedule,
            DefaultLocation = program.DefaultLocation,
            EnrolledCount = summary?.EnrolledCount ?? 0,
            AttendancePct = summary?.AttendancePct,
            Participants = participants.Select(p => new ParticipantSummaryDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Initials = p.Initials,
                Status = p.Status,
                ProgramId = p.ProgramId,
                ProgramName = program.Name,
                AttendancePct = p.AttendancePct,
                StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                HasDocAlerts = false,
            }).ToList(),
            UpcomingEvents = events.Select(e => new CalendarEventDto
            {
                Id = e.Id,
                Title = e.Title,
                Location = e.Location,
                Meta = e.Meta,
                Date = e.Date.ToString("yyyy-MM-dd"),
                TimeRange = e.TimeRange,
                ProgramId = e.ProgramId,
                ProgramName = program.Name,
                IsUpcoming = e.IsUpcoming,
            }).ToList(),
            Staff = new(),
            Alerts = participants
                .Where(p => p.Status == Domain.Enums.ParticipantStatus.Attention)
                .Select(p => new ProgramAlertDto
                {
                    Severity = "Warning",
                    Message = $"{p.FullName} needs attention",
                }).ToList(),
        };
    }

    public async Task<ProgramSummaryDto> CreateAsync(CreateProgramDto dto)
    {
        var program = new CrmProgram
        {
            Name = dto.Name,
            Slug = dto.Name.ToLower()
                        .Replace(" ", "-")
                        .Replace("'", "")
                        .Replace(".", ""),
            ColorHex = dto.ColorHex,
            SessionSchedule = dto.SessionSchedule,
            DefaultLocation = dto.DefaultLocation,
        };

        await _uow.Programs.AddAsync(program);
        await _uow.SaveChangesAsync();

        return new ProgramSummaryDto
        {
            Id = program.Id,
            Name = program.Name,
            Slug = program.Slug,
            ColorHex = program.ColorHex,
            SessionSchedule = program.SessionSchedule,
            DefaultLocation = program.DefaultLocation,
            EnrolledCount = 0,
            AlertCount = 0,
        };
    }
}
