using CRM.Domain.Common;

namespace CRM.Domain.Entities;

// Master onboarding checklist. Copied onto each new staff member at creation;
// editing the template does not touch checklists already issued.
public class ChecklistTemplateItem : BaseEntity
{
    public string Section { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
