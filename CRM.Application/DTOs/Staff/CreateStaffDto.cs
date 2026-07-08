using System.ComponentModel.DataAnnotations;
using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Staff;

public class CreateStaffDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string Initials { get; set; } = string.Empty;

    public StaffRole Role { get; set; } = StaffRole.Teacher;
    public DateTime? StartDate { get; set; }
    public List<Guid> ProgramIds { get; set; } = new();
}

public class UpdateStaffDto
{
    [StringLength(200, MinimumLength = 1)]
    public string? FullName { get; set; }

    [StringLength(10, MinimumLength = 1)]
    public string? Initials { get; set; }

    public StaffRole? Role { get; set; }
    public List<Guid>? ProgramIds { get; set; }
}
