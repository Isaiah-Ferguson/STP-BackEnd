using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Participants;

public class ParticipantSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public ParticipantStatus Status { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string ProgramSlug { get; set; } = string.Empty;
    public int AttendancePct { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public bool HasDocAlerts { get; set; }
    public int? BirthYear { get; set; }
    public string? ServiceCoordinator { get; set; }
}
