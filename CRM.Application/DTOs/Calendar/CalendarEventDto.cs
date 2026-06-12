namespace CRM.Application.DTOs.Calendar;

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Meta { get; set; }
    public string Date { get; set; } = string.Empty;
    public string? TimeRange { get; set; }
    public Guid? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public bool IsUpcoming { get; set; }
}

public class CreateCalendarEventDto
{
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public Guid? ProgramId { get; set; }
    public string? Location { get; set; }
    public string? Meta { get; set; }
    public string? TimeRange { get; set; }
}
