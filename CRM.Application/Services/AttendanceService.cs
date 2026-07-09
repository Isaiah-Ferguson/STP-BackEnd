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

    public async Task<AttendanceSessionDto?> GetSessionAsync(Guid userId, Guid sessionId)
    {
        var session = await _uow.Sessions.GetByIdAsync(sessionId);
        if (session is null) return null;

        await EnsureAssignedToProgramAsync(userId, session.ProgramId);

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

    public async Task<bool> UpdateRecordAsync(Guid userId, Guid recordId, UpdateAttendanceDto dto)
    {
        var record = await _uow.Attendance.GetByIdAsync(recordId);
        if (record is null) return false;

        var session = await _uow.Sessions.GetByIdAsync(record.SessionId);

        // Only teachers assigned to the record's program (or an Admin) may edit it.
        if (session is not null)
            await EnsureAssignedToProgramAsync(userId, session.ProgramId);

        // A submitted session is finalized — its records are locked.
        if (session is not null && session.Status == SessionStatus.Submitted)
            throw new InvalidOperationException("This session has been submitted and can no longer be edited.");

        record.Status = dto.Status;
        await _uow.Attendance.UpdateAsync(record);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<ScheduledSessionDto>> GetScheduledForUserAsync(Guid userId, DateTime date)
    {
        var day = date.Date;
        var nextDay = day.AddDays(1);
        var todayFlag = ToFlag(day.DayOfWeek);

        var (isAdmin, staffMemberId) = await ResolveUserAsync(userId);
        var programs = await _uow.Programs.GetAllAsync();
        var allowed = await AllowedProgramIdsAsync(isAdmin, staffMemberId, programs);
        if (allowed.Count == 0) return new List<ScheduledSessionDto>();

        var activeByProgram = (await _uow.Participants.ListAsync(p => p.Status == ParticipantStatus.Active))
            .Where(p => allowed.Contains(p.ProgramId))
            .GroupBy(p => p.ProgramId)
            .ToDictionary(g => g.Key, g => g.Count());

        var sessionByProgram = (await _uow.Sessions.ListAsync(s => s.Date >= day && s.Date < nextDay))
            .Where(s => allowed.Contains(s.ProgramId))
            .GroupBy(s => s.ProgramId)
            .ToDictionary(g => g.Key, g => g.First());

        var sessionIds = sessionByProgram.Values.Select(s => s.Id).ToHashSet();
        var markedBySession = (sessionIds.Count == 0
                ? new List<AttendanceRecord>()
                : (await _uow.Attendance.ListAsync(r => sessionIds.Contains(r.SessionId))).ToList())
            .GroupBy(r => r.SessionId)
            .ToDictionary(g => g.Key, g => g.Count(r => r.Status != AttendanceStatus.Unmarked));

        var cards = new List<ScheduledSessionDto>();
        foreach (var program in programs.Where(p => allowed.Contains(p.Id)))
        {
            var meetsToday = todayFlag != MeetingDays.None && program.MeetingDays.HasFlag(todayFlag);
            sessionByProgram.TryGetValue(program.Id, out var session);

            // Show a program only if it's scheduled today, or already has a session started today.
            if (!meetsToday && session is null) continue;

            var marked = session is not null ? markedBySession.GetValueOrDefault(session.Id, 0) : 0;
            cards.Add(new ScheduledSessionDto
            {
                SessionId = session?.Id,
                ProgramId = program.Id,
                ProgramSlug = program.Slug,
                ProgramName = program.Name,
                ColorHex = program.ColorHex,
                Date = day.ToString("yyyy-MM-dd"),
                TimeRange = session?.TimeRange ?? FormatTimeRange(program.StartTime, program.EndTime),
                Room = session?.Room ?? program.DefaultLocation,
                // "in-progress" only once at least one record is marked — an empty session
                // (or no session yet) reads as "not-started".
                Status = session?.Status == SessionStatus.Submitted
                    ? "submitted"
                    : marked > 0 ? "in-progress" : "not-started",
                MarkedCount = marked,
                TotalCount = activeByProgram.GetValueOrDefault(program.Id, 0),
                IsAdHoc = session is not null && !meetsToday,
            });
        }

        return cards.OrderBy(c => c.ProgramName).ToList();
    }

    public async Task<SessionRosterDto?> GetOrCreateSessionAsync(Guid userId, Guid programId, DateTime date)
    {
        var day = date.Date;
        var nextDay = day.AddDays(1);

        var program = await _uow.Programs.GetByIdAsync(programId);
        if (program is null) return null;

        var (isAdmin, staffMemberId) = await ResolveUserAsync(userId);
        var allowed = await AllowedProgramIdsAsync(isAdmin, staffMemberId, new[] { program });
        if (!allowed.Contains(programId))
            throw new UnauthorizedAccessException("You are not assigned to this program.");

        var session = (await _uow.Sessions.ListAsync(s => s.ProgramId == programId && s.Date >= day && s.Date < nextDay))
            .FirstOrDefault();
        if (session is null)
        {
            session = new Session
            {
                ProgramId = programId,
                Date = day,
                Room = program.DefaultLocation,
                TimeRange = FormatTimeRange(program.StartTime, program.EndTime),
                Status = SessionStatus.Open,
            };
            await _uow.Sessions.AddAsync(session);
            await _uow.SaveChangesAsync();
        }

        var participants = await _uow.Participants.ListAsync(
            p => p.ProgramId == programId && p.Status == ParticipantStatus.Active);

        var recordByParticipant = (await _uow.Attendance.ListAsync(r => r.SessionId == session.Id))
            .GroupBy(r => r.ParticipantId)
            .ToDictionary(g => g.Key, g => g.First());

        var created = false;
        foreach (var p in participants)
        {
            if (recordByParticipant.ContainsKey(p.Id)) continue;
            var rec = new AttendanceRecord
            {
                ParticipantId = p.Id,
                SessionId = session.Id,
                Status = AttendanceStatus.Unmarked,
            };
            await _uow.Attendance.AddAsync(rec);
            recordByParticipant[p.Id] = rec;
            created = true;
        }
        if (created) await _uow.SaveChangesAsync();

        return await BuildSessionRosterAsync(program, session, participants, recordByParticipant);
    }

    public async Task<SessionRosterDto?> GetProgramSessionReadOnlyAsync(Guid userId, Guid programId, DateTime date)
    {
        var day = date.Date;
        var nextDay = day.AddDays(1);

        var program = await _uow.Programs.GetByIdAsync(programId);
        if (program is null) return null;

        var (isAdmin, staffMemberId) = await ResolveUserAsync(userId);
        var allowed = await AllowedProgramIdsAsync(isAdmin, staffMemberId, new[] { program });
        if (!allowed.Contains(programId))
            throw new UnauthorizedAccessException("You are not assigned to this program.");

        // Read-only (#23): a GET must never create sessions or records — prefetches and
        // crawlers were able to open sessions for programs that never met.
        var session = (await _uow.Sessions.ListAsync(s => s.ProgramId == programId && s.Date >= day && s.Date < nextDay))
            .FirstOrDefault();
        if (session is null) return null;

        var participants = await _uow.Participants.ListAsync(
            p => p.ProgramId == programId && p.Status == ParticipantStatus.Active);

        var recordByParticipant = (await _uow.Attendance.ListAsync(r => r.SessionId == session.Id))
            .GroupBy(r => r.ParticipantId)
            .ToDictionary(g => g.Key, g => g.First());

        return await BuildSessionRosterAsync(program, session, participants, recordByParticipant);
    }

    /// <summary>Assembles the roster DTO from already-loaded session/participants/records. Loads notes.</summary>
    private async Task<SessionRosterDto> BuildSessionRosterAsync(
        CrmProgram program,
        Session session,
        IReadOnlyList<Participant> participants,
        Dictionary<Guid, AttendanceRecord> recordByParticipant)
    {
        var recordIds = recordByParticipant.Values.Select(r => r.Id).ToHashSet();
        var notesByRecord = (recordIds.Count == 0
                ? new List<AttendanceNote>()
                : (await _uow.AttendanceNotes.ListAsync(n => recordIds.Contains(n.AttendanceRecordId))).ToList())
            .GroupBy(n => n.AttendanceRecordId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var entries = participants
            .Where(p => recordByParticipant.ContainsKey(p.Id))
            .Select(p =>
            {
                var rec = recordByParticipant[p.Id];
                return new AttendanceRosterEntryDto
                {
                    RecordId = rec.Id,
                    ParticipantId = p.Id,
                    FullName = p.FullName,
                    Initials = p.Initials,
                    ProgramId = program.Id,
                    ProgramSlug = program.Slug,
                    ProgramName = program.Name,
                    Status = rec.Status,
                    Notes = notesByRecord.GetValueOrDefault(rec.Id, new())
                        .Select(n => new AttendanceNoteDto { Id = n.Id, Content = n.Content, NoteType = n.NoteType })
                        .ToList(),
                };
            })
            .OrderBy(e => e.FullName)
            .ToList();

        return new SessionRosterDto
        {
            SessionId = session.Id,
            ProgramId = program.Id,
            ProgramSlug = program.Slug,
            ProgramName = program.Name,
            ColorHex = program.ColorHex,
            Date = session.Date.ToString("yyyy-MM-dd"),
            TimeRange = session.TimeRange,
            Room = session.Room,
            Status = session.Status == SessionStatus.Submitted ? "submitted" : "open",
            SubmittedAt = session.SubmittedAt,
            Entries = entries,
        };
    }

    public async Task<bool> SubmitSessionAsync(Guid userId, Guid sessionId)
    {
        var session = await _uow.Sessions.GetByIdAsync(sessionId);
        if (session is null) return false;

        var (isAdmin, staffMemberId) = await ResolveUserAsync(userId);
        var allowed = await AllowedProgramIdsAsync(isAdmin, staffMemberId, await _uow.Programs.GetAllAsync());
        if (!allowed.Contains(session.ProgramId))
            throw new UnauthorizedAccessException("You are not assigned to this program.");

        session.Status = SessionStatus.Submitted;
        session.SubmittedAt = DateTime.UtcNow;
        await _uow.Sessions.UpdateAsync(session);
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

    public async Task<IReadOnlyList<AttendanceRosterEntryDto>> GetTodayRosterReadOnlyAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var todaySessions = await _uow.Sessions.ListAsync(s => s.Date >= today && s.Date < tomorrow, ct);
        var sessionIds = todaySessions.Select(s => s.Id).ToHashSet();
        if (sessionIds.Count == 0) return new List<AttendanceRosterEntryDto>();

        var records = await _uow.Attendance.ListAsync(r => sessionIds.Contains(r.SessionId));
        if (records.Count == 0) return new List<AttendanceRosterEntryDto>();

        var participantIds = records.Select(r => r.ParticipantId).ToHashSet();
        var participants = (await _uow.Participants.ListAsync(p => participantIds.Contains(p.Id)))
            .ToDictionary(p => p.Id);
        var programs = (await _uow.Programs.GetAllAsync()).ToDictionary(p => p.Id);

        return records
            .Where(r => participants.ContainsKey(r.ParticipantId))
            .Select(r =>
            {
                var p = participants[r.ParticipantId];
                var prog = programs.GetValueOrDefault(p.ProgramId);
                return new AttendanceRosterEntryDto
                {
                    RecordId = r.Id,
                    ParticipantId = p.Id,
                    FullName = p.FullName,
                    Initials = p.Initials,
                    ProgramId = p.ProgramId,
                    ProgramSlug = prog?.Slug ?? string.Empty,
                    ProgramName = prog?.Name ?? string.Empty,
                    Status = r.Status,
                    Notes = new(),
                };
            })
            .OrderBy(e => e.ProgramName)
            .ThenBy(e => e.FullName)
            .ToList();
    }

    public async Task<AttendanceNoteDto?> AddNoteAsync(Guid userId, Guid recordId, CreateAttendanceNoteDto dto)
    {
        var record = await _uow.Attendance.GetByIdAsync(recordId);
        if (record is null) return null;

        var session = await _uow.Sessions.GetByIdAsync(record.SessionId);
        if (session is not null)
            await EnsureAssignedToProgramAsync(userId, session.ProgramId);

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

    // ── Helpers ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Throws <see cref="UnauthorizedAccessException"/> unless the user is an Admin or a staff
    /// member assigned to <paramref name="programId"/>. Prevents one teacher from reading or
    /// editing another program's attendance by record/session GUID (IDOR, #5).
    /// </summary>
    private async Task EnsureAssignedToProgramAsync(Guid userId, Guid programId)
    {
        var (isAdmin, staffMemberId) = await ResolveUserAsync(userId);
        if (isAdmin) return;

        var assignments = await _uow.GetStaffProgramAssignmentsAsync();
        var assigned = staffMemberId is { } sid
            && assignments.Any(a => a.StaffMemberId == sid && a.ProgramId == programId);
        if (!assigned)
            throw new UnauthorizedAccessException("You are not assigned to this program.");
    }

    /// <summary>Resolves the calling user to (isAdmin, linked staff id) for program scoping.</summary>
    private async Task<(bool IsAdmin, Guid? StaffMemberId)> ResolveUserAsync(Guid userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user is null) return (false, null);
        return (user.Role == UserRole.Admin, user.StaffMemberId);
    }

    /// <summary>
    /// The set of program ids the user may take attendance for: every program for an Admin,
    /// otherwise the programs their linked staff record is assigned to.
    /// </summary>
    private async Task<HashSet<Guid>> AllowedProgramIdsAsync(
        bool isAdmin, Guid? staffMemberId, IReadOnlyList<CrmProgram> programs)
    {
        if (isAdmin) return programs.Select(p => p.Id).ToHashSet();
        if (staffMemberId is null) return new HashSet<Guid>();

        var assignments = await _uow.GetStaffProgramAssignmentsAsync();
        return assignments
            .Where(a => a.StaffMemberId == staffMemberId.Value)
            .Select(a => a.ProgramId)
            .ToHashSet();
    }

    private static MeetingDays ToFlag(DayOfWeek d) => d switch
    {
        DayOfWeek.Sunday => MeetingDays.Sunday,
        DayOfWeek.Monday => MeetingDays.Monday,
        DayOfWeek.Tuesday => MeetingDays.Tuesday,
        DayOfWeek.Wednesday => MeetingDays.Wednesday,
        DayOfWeek.Thursday => MeetingDays.Thursday,
        DayOfWeek.Friday => MeetingDays.Friday,
        DayOfWeek.Saturday => MeetingDays.Saturday,
        _ => MeetingDays.None,
    };

    private static string? FormatTimeRange(TimeOnly? start, TimeOnly? end)
    {
        if (start is null && end is null) return null;
        if (start is not null && end is not null) return $"{start:h:mm tt}–{end:h:mm tt}";
        return (start ?? end)?.ToString("h:mm tt");
    }
}
