using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Programs;

/// <summary>
/// Full update of a program's editable fields. The slug is intentionally immutable
/// (it backs routes and links), so it is not part of this DTO.
/// </summary>
public class UpdateProgramDto
{
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string? SessionSchedule { get; set; }
    public string? DefaultLocation { get; set; }
    public MeetingDays MeetingDays { get; set; } = MeetingDays.None;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}
