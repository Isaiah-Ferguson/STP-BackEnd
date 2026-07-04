namespace CRM.Application.DTOs.Roster;

/// <summary>
/// One participant's roster row for a term: who they are, their program, and their
/// (possibly not-yet-set) Site / StarGroup / assigned-staff placement. Participants
/// with no assignment for the term still appear, with the assignment fields null.
/// </summary>
public class RosterEntryDto
{
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string ParticipantInitials { get; set; } = string.Empty;
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string ProgramSlug { get; set; } = string.Empty;

    public Guid? AssignmentId { get; set; }
    public Guid? SiteId { get; set; }
    public string? SiteName { get; set; }
    public Guid? StarGroupId { get; set; }
    public string? StarGroupName { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public string? AssignedStaffName { get; set; }
    public bool CountedInRatio { get; set; } = true;
    public string? Notes { get; set; }

    public int Quarter { get; set; }
    public int Year { get; set; }
}

public class UpsertRosterAssignmentDto
{
    public Guid ParticipantId { get; set; }
    public int Quarter { get; set; }
    public int Year { get; set; }
    public Guid? SiteId { get; set; }
    public Guid? StarGroupId { get; set; }
    public Guid? AssignedStaffId { get; set; }
    public bool CountedInRatio { get; set; } = true;
    public string? Notes { get; set; }
}
