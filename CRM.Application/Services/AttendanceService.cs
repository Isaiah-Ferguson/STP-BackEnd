using CRM.Application.DTOs.Attendance;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _uow;

    public AttendanceService(IUnitOfWork uow) => _uow = uow;

    public async Task<AttendanceSessionDto?> GetSessionAsync(Guid sessionId)
    {
        var session = await _uow.Sessions.GetByIdAsync(sessionId);
        if (session is null) return null;

        var records = await _uow.Attendance.ListAsync(r => r.SessionId == sessionId);

        var participantIds = records.Select(r => r.ParticipantId).ToHashSet();
        var participants = await _uow.Participants.ListAsync(p => participantIds.Contains(p.Id));
        var participantMap = participants.ToDictionary(p => p.Id);

        return new AttendanceSessionDto
        {
            SessionId = session.Id,
            ProgramId = session.ProgramId,
            Date = session.Date.ToString("yyyy-MM-dd"),
            Room = session.Room,
            TimeRange = session.TimeRange,
            Records = records.Select(r =>
            {
                var participant = participantMap.GetValueOrDefault(r.ParticipantId);
                return new AttendanceRecordDto
                {
                    Id = r.Id,
                    ParticipantId = r.ParticipantId,
                    ParticipantName = participant?.FullName ?? string.Empty,
                    ParticipantInitials = participant?.Initials ?? string.Empty,
                    Status = r.Status,
                    Group = r.Group,
                    Notes = new(),
                };
            }).ToList(),
        };
    }

    public async Task<bool> UpdateRecordAsync(Guid recordId, UpdateAttendanceDto dto)
    {
        var record = await _uow.Attendance.GetByIdAsync(recordId);
        if (record is null) return false;

        record.Status = dto.Status;
        await _uow.Attendance.UpdateAsync(record);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<AttendanceRosterEntryDto>> GetTodayRosterAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var programs = await _uow.Programs.GetAllAsync();
        var participants = await _uow.Participants.ListAsync(p => p.Status == ParticipantStatus.Active);

        // One session per program per day — create any that are missing.
        var sessions = (await _uow.Sessions.ListAsync(s => s.Date >= today && s.Date < tomorrow)).ToList();
        var sessionByProgram = sessions
            .GroupBy(s => s.ProgramId)
            .ToDictionary(g => g.Key, g => g.First());

        var createdSession = false;
        foreach (var programId in participants.Select(p => p.ProgramId).Distinct())
        {
            if (sessionByProgram.ContainsKey(programId)) continue;
            var session = new Session { ProgramId = programId, Date = today };
            await _uow.Sessions.AddAsync(session);
            sessionByProgram[programId] = session;
            createdSession = true;
        }
        if (createdSession) await _uow.SaveChangesAsync();

        // One record per active participant for today's session — create missing ones (Unmarked).
        var sessionIds = sessionByProgram.Values.Select(s => s.Id).ToHashSet();
        var records = (await _uow.Attendance.ListAsync(r => sessionIds.Contains(r.SessionId))).ToList();
        var recordByParticipant = records
            .GroupBy(r => r.ParticipantId)
            .ToDictionary(g => g.Key, g => g.First());

        var createdRecord = false;
        foreach (var p in participants)
        {
            if (recordByParticipant.ContainsKey(p.Id)) continue;
            if (!sessionByProgram.TryGetValue(p.ProgramId, out var session)) continue;
            var record = new AttendanceRecord
            {
                ParticipantId = p.Id,
                SessionId = session.Id,
                Status = AttendanceStatus.Unmarked,
            };
            await _uow.Attendance.AddAsync(record);
            recordByParticipant[p.Id] = record;
            createdRecord = true;
        }
        if (createdRecord) await _uow.SaveChangesAsync();

        // Notes for today's records.
        var recordIds = recordByParticipant.Values.Select(r => r.Id).ToHashSet();
        var notes = recordIds.Count == 0
            ? new List<AttendanceNote>()
            : (await _uow.AttendanceNotes.ListAsync(n => recordIds.Contains(n.AttendanceRecordId))).ToList();
        var notesByRecord = notes
            .GroupBy(n => n.AttendanceRecordId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var programMap = programs.ToDictionary(p => p.Id);

        return participants
            .Where(p => recordByParticipant.ContainsKey(p.Id))
            .Select(p =>
            {
                var record = recordByParticipant[p.Id];
                var program = programMap.GetValueOrDefault(p.ProgramId);
                return new AttendanceRosterEntryDto
                {
                    RecordId = record.Id,
                    ParticipantId = p.Id,
                    FullName = p.FullName,
                    Initials = p.Initials,
                    ProgramId = p.ProgramId,
                    ProgramSlug = program?.Slug ?? string.Empty,
                    ProgramName = program?.Name ?? string.Empty,
                    Status = record.Status,
                    Notes = notesByRecord.GetValueOrDefault(record.Id, new())
                        .Select(n => new AttendanceNoteDto { Id = n.Id, Content = n.Content, NoteType = n.NoteType })
                        .ToList(),
                };
            })
            .OrderBy(e => e.ProgramName)
            .ThenBy(e => e.FullName)
            .ToList();
    }

    public async Task<AttendanceNoteDto?> AddNoteAsync(Guid recordId, CreateAttendanceNoteDto dto)
    {
        var record = await _uow.Attendance.GetByIdAsync(recordId);
        if (record is null) return null;

        var note = new AttendanceNote
        {
            AttendanceRecordId = recordId,
            Content = dto.Content.Trim(),
            NoteType = dto.NoteType,
        };
        await _uow.AttendanceNotes.AddAsync(note);
        await _uow.SaveChangesAsync();

        return new AttendanceNoteDto { Id = note.Id, Content = note.Content, NoteType = note.NoteType };
    }
}
