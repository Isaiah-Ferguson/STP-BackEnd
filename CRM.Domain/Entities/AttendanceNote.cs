using CRM.Domain.Common;

namespace CRM.Domain.Entities;

public class AttendanceNote : BaseEntity
{
    public Guid AttendanceRecordId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string NoteType { get; set; } = "observation";

    public AttendanceRecord AttendanceRecord { get; set; } = null!;
}
