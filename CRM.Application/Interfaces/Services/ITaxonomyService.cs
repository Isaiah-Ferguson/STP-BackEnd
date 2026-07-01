using CRM.Application.DTOs.Taxonomy;

namespace CRM.Application.Interfaces.Services;

public interface ITaxonomyService
{
    /// <summary>The six objective areas, ordered, each with its nested sub-skills.</summary>
    Task<IReadOnlyList<ObjectiveAreaDto>> GetObjectiveAreasAsync();

    /// <summary>All sub-skills, flat, ordered by section then sort order.</summary>
    Task<IReadOnlyList<SubSkillDto>> GetSubSkillsAsync();

    /// <summary>The reference dropdown sources (objective areas, sub-skills, progress levels).</summary>
    Task<ReferenceListsDto> GetListsAsync();
}
