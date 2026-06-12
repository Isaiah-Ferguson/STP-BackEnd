using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid SessionId { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Unmarked;
    public string? Group { get; set; }

    public Participant Participant { get; set; } = null!;
    public Session Session { get; set; } = null!;
    public ICollection<AttendanceNote> Notes { get; set; } = new List<AttendanceNote>();
}
