using CRM.Application.DTOs.Games;

namespace CRM.Application.Interfaces.Services;

public interface IGameBacklogService
{
    Task<IReadOnlyList<GameIdeaDto>> GetIdeasAsync();
    Task<GameIdeaDto> CreateIdeaAsync(CreateGameIdeaDto dto);
    /// <summary>Promotes an idea into a draft Games Library entry (Suggested, Multi-Area, all tiers) to be refined. Null if the idea doesn't exist.</summary>
    Task<GameIdeaDto?> PromoteIdeaAsync(Guid id);

    Task<IReadOnlyList<AgeModificationDto>> GetAgeModsAsync();
    Task<AgeModificationDto> CreateAgeModAsync(CreateAgeModificationDto dto);
}
