using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Participants;

public class CreateParticipantDto
{
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public Guid ProgramId { get; set; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Active;
    public int? BirthYear { get; set; }
    public string? ServiceCoordinator { get; set; }
    public DateTime? StartDate { get; set; }
}

public class UpdateParticipantDto
{
    public string? FullName { get; set; }
    public string? Initials { get; set; }
    public Guid? ProgramId { get; set; }
    public ParticipantStatus? Status { get; set; }
    public int? BirthYear { get; set; }
    public string? ServiceCoordinator { get; set; }
}
