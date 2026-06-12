using CRM.Domain.Common;

namespace CRM.Domain.Entities;

public class DocumentRecord : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public bool IsComplete { get; set; }

    public Participant Participant { get; set; } = null!;
}
