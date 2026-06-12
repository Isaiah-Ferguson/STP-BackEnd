namespace CRM.Application.DTOs.Programs;

public class CreateProgramDto
{
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string? SessionSchedule { get; set; }
    public string? DefaultLocation { get; set; }
}
