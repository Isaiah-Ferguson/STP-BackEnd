using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Project : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public ProjectType Type { get; set; } = ProjectType.Production;
    public string Status { get; set; } = "planning";
    public string? Scope { get; set; }
    public DateTime? DueDate { get; set; }

    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
