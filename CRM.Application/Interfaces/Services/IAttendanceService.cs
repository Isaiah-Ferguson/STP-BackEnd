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
    Task<IReadOnlyList<AttendanceRosterEntryDto>> GetTodayRosterAsync();

    /// <summary>Adds a note to an attendance record. Returns null if the record is not found.</summary>
    Task<AttendanceNoteDto?> AddNoteAsync(Guid recordId, CreateAttendanceNoteDto dto);
}
