using CRM.Domain.Common;

namespace CRM.Domain.Entities;

public class CrmProgram : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string? DefaultLocation { get; set; }
    public string? SessionSchedule { get; set; }

    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public ICollection<StaffProgramAssignment> StaffAssignments { get; set; } = new List<StaffProgramAssignment>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
    public ICollection<ScriptProgram> ScriptPrograms { get; set; } = new List<ScriptProgram>();
}
