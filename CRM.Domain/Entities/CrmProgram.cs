using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class CrmProgram : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string? DefaultLocation { get; set; }

    /// <summary>Human-readable schedule label for display, e.g. "Mon / Wed / Fri".</summary>
    public string? SessionSchedule { get; set; }

    /// <summary>Structured days the program meets — drives which programs surface for a given date.</summary>
    public MeetingDays MeetingDays { get; set; } = MeetingDays.None;

    /// <summary>Default meeting start/end time, used to stamp new sessions and for display.</summary>
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public ICollection<StaffProgramAssignment> StaffAssignments { get; set; } = new List<StaffProgramAssignment>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
    public ICollection<ScriptProgram> ScriptPrograms { get; set; } = new List<ScriptProgram>();
}
