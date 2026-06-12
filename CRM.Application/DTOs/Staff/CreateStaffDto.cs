using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Staff;

public class CreateStaffDto
{
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public StaffRole Role { get; set; } = StaffRole.Teacher;
    public DateTime? StartDate { get; set; }
    public List<Guid> ProgramIds { get; set; } = new();
}

public class UpdateStaffDto
{
    public string? FullName { get; set; }
    public string? Initials { get; set; }
    public StaffRole? Role { get; set; }
    public List<Guid>? ProgramIds { get; set; }
}
