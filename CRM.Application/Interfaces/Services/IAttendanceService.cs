using CRM.Application.DTOs.Attendance;

namespace CRM.Application.Interfaces.Services;

public interface IAttendanceService
{
    Task<AttendanceSessionDto?> GetSessionAsync(Guid sessionId);
    Task<bool> UpdateRecordAsync(Guid recordId, UpdateAttendanceDto dto);

    /// <summary>
    /// Returns today's attendance roster across all programs, lazily creating the
    /// per-program session and a record for each active participant if missing.
    /// </summary>
    [Obsolete("Superseded by GetScheduledForUserAsync + GetOrCreateSessionAsync; kept until the frontend migrates.")]
    Task<IReadOnlyList<AttendanceRosterEntryDto>> GetTodayRosterAsync();

    /// <summary>
    /// The session cards for a given date, scoped to the user's programs (all programs
    /// for an Admin). Includes programs scheduled to meet that day plus any program
    /// with a session already started that day (ad-hoc). Does not create anything.
    /// </summary>
    Task<IReadOnlyList<ScheduledSessionDto>> GetScheduledForUserAsync(Guid userId, DateTime date);

    /// <summary>
    /// Gets — or lazily creates — the session for a single program on a date, returning its
    /// roster of active participants. Returns null if the program does not exist; throws
    /// <see cref="UnauthorizedAccessException"/> if the user is not assigned to the program.
    /// </summary>
    Task<SessionRosterDto?> GetOrCreateSessionAsync(Guid userId, Guid programId, DateTime date);

    /// <summary>
    /// Finalizes a session: marks it Submitted and locks its records. Returns false if the
    /// session is not found; throws <see cref="UnauthorizedAccessException"/> if the user is
    /// not assigned to the session's program.
    /// </summary>
    Task<bool> SubmitSessionAsync(Guid userId, Guid sessionId);

    /// <summary>
    /// Today's roster from existing sessions/records only — no lazy creation, no writes.
    /// Returns an empty list if no session exists for today. Intended for read-only views
    /// like the dashboard.
    /// </summary>
    Task<IReadOnlyList<AttendanceRosterEntryDto>> GetTodayRosterReadOnlyAsync();

    /// <summary>Adds a note to an attendance record. Returns null if the record is not found.</summary>
    Task<AttendanceNoteDto?> AddNoteAsync(Guid recordId, CreateAttendanceNoteDto dto);
}
