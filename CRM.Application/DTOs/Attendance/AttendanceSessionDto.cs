using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Attendance;

public class AttendanceSessionDto
{
    public Guid SessionId { get; set; }
    public Guid ProgramId { get; set; }
    public string Date { get; set; } = string.Empty;
    public string? Room { get; set; }
    public string? TimeRange { get; set; }
    public List<AttendanceRecordDto> Records { get; set; } = new();
}

public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string ParticipantInitials { get; set; } = string.Empty;
    public AttendanceStatus Status { get; set; }
    public string? Group { get; set; }
    public List<AttendanceNoteDto> Notes { get; set; } = new();
}

public class AttendanceNoteDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string NoteType { get; set; } = string.Empty;
}

public class UpdateAttendanceDto
{
    public AttendanceStatus Status { get; set; }
}
