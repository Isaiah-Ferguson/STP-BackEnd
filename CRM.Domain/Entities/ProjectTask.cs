using CRM.Domain.Common;
using CRM.Domain.Enums;
using TaskStatus = CRM.Domain.Enums.TaskStatus;

namespace CRM.Domain.Entities;

public class ProjectTask : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Context { get; set; }
    public Guid? AssignedToId { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Upcoming;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public bool IsOverdue { get; set; }

    public Project Project { get; set; } = null!;
    public StaffMember? AssignedTo { get; set; }
}
