using System.ComponentModel.DataAnnotations;
using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Participants;

public class CreateParticipantDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string Initials { get; set; } = string.Empty;

    [Required]
    public Guid ProgramId { get; set; }

    public ParticipantStatus Status { get; set; } = ParticipantStatus.Active;

    [Range(1900, 2100)]
    public int? BirthYear { get; set; }

    [StringLength(200)]
    public string? ServiceCoordinator { get; set; }

    public DateTime? StartDate { get; set; }
}

public class UpdateParticipantDto
{
    [StringLength(200, MinimumLength = 1)]
    public string? FullName { get; set; }

    [StringLength(10, MinimumLength = 1)]
    public string? Initials { get; set; }

    public Guid? ProgramId { get; set; }
    public ParticipantStatus? Status { get; set; }

    [Range(1900, 2100)]
    public int? BirthYear { get; set; }

    [StringLength(200)]
    public string? ServiceCoordinator { get; set; }
}
