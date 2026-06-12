using CRM.Application.DTOs.Attendance;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;

namespace CRM.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _uow;

    public AttendanceService(IUnitOfWork uow) => _uow = uow;

    public async Task<AttendanceSessionDto?> GetSessionAsync(Guid sessionId)
    {
        var session = await _uow.Sessions.GetByIdAsync(sessionId);
        if (session is null) return null;

        var records = (await _uow.Attendance.GetAllAsync())
            .Where(r => r.SessionId == sessionId).ToList();

        var participants = await _uow.Participants.GetAllAsync();
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
}
