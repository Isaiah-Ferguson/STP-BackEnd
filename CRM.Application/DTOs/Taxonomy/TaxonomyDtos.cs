namespace CRM.Application.DTOs.Taxonomy;

public class ObjectiveAreaDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<SubSkillDto> SubSkills { get; set; } = new();
}

public class SubSkillDto
{
    public Guid Id { get; set; }
    public Guid ObjectiveAreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int SectionNumber { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ObjectiveAreaName { get; set; }
    public string? ObjectiveAreaColorHex { get; set; }
}

/// <summary>
/// The reference dropdown sources mirrored from the spreadsheets' "Do Not Remove-
/// Lists" tabs: the objective areas (with their nested sub-skills) and the progress
/// levels. Extended in later phases with staff, sites, and star groups.
/// </summary>
public class ReferenceListsDto
{
    public List<ObjectiveAreaDto> ObjectiveAreas { get; set; } = new();
    public List<SubSkillDto> SubSkills { get; set; } = new();
    public List<string> ProgressLevels { get; set; } = new();
}
