using CRM.Domain.Common;

namespace CRM.Domain.Entities;

public class Session : BaseEntity
{
    public Guid ProgramId { get; set; }
    public DateTime Date { get; set; }
    public string? Room { get; set; }
    public string? TimeRange { get; set; }
    public string? Label { get; set; }

    public CrmProgram Program { get; set; } = null!;
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
}
