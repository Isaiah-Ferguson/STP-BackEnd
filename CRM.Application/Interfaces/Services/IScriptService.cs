using CRM.Application.DTOs.Scripts;

namespace CRM.Application.Interfaces.Services;

public interface IScriptService
{
    Task<IReadOnlyList<ScriptDto>> GetAllAsync();
    Task<ScriptDto?> GetByIdAsync(Guid id);
    Task<ScriptDto> CreateAsync(CreateScriptDto dto);
    Task<ScriptDto?> UpdateAsync(Guid id, UpdateScriptDto dto);
}
