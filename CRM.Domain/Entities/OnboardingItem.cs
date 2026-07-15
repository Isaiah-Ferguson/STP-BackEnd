using CRM.Domain.Common;

namespace CRM.Domain.Entities;

public class OnboardingItem : BaseEntity
{
    public Guid StaffMemberId { get; set; }
    public string Section { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public StaffMember StaffMember { get; set; } = null!;
}
