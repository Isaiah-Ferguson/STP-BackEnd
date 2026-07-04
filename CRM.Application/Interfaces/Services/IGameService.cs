using CRM.Application.DTOs.Games;

namespace CRM.Application.Interfaces.Services;

public interface IGameService
{
    Task<IReadOnlyList<GameSummaryDto>> QueryAsync(GameFilter filter);
    Task<GameDetailDto?> GetByIdAsync(Guid id);
    Task<GameDetailDto> CreateAsync(CreateGameDto dto);
    Task<GameDetailDto?> UpdateAsync(Guid id, UpdateGameDto dto);
}
