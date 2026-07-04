using CRM.Domain.Common;

namespace CRM.Domain.Entities;

/// <summary>
/// A physical program site (MJC Modesto, Manteca, Pathways) used by the roster. A
/// first-class lookup distinct from <see cref="CrmProgram"/> so a site can host
/// several groups. Reference data, seeded once via migration.
/// </summary>
public class Site : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
