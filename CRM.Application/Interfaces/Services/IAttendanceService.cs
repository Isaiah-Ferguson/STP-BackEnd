using CRM.Application.DTOs.Attendance;

namespace CRM.Application.Interfaces.Services;

public interface IAttendanceService
{
    Task<AttendanceSessionDto?> GetSessionAsync(Guid sessionId);
    Task<bool> UpdateRecordAsync(Guid recordId, UpdateAttendanceDto dto);
}
