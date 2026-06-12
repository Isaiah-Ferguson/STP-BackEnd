using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Participant : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public int? BirthYear { get; set; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Active;
    public Guid ProgramId { get; set; }
    public string? ServiceCoordinator { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public int AttendancePct { get; set; }

    public CrmProgram Program { get; set; } = null!;
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<DocumentRecord> Documents { get; set; } = new List<DocumentRecord>();
}
