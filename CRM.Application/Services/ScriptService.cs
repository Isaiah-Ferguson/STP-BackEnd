using CRM.Application.DTOs.Scripts;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class ScriptService : IScriptService
{
    private readonly IUnitOfWork _uow;

    public ScriptService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ScriptDto>> GetAllAsync()
    {
        var scripts = await _uow.Scripts.GetAllAsync();
        return scripts.Select(s => ToDto(s)).ToList();
    }

    public async Task<ScriptDto?> GetByIdAsync(Guid id)
    {
        var script = await _uow.Scripts.GetByIdAsync(id);
        return script is null ? null : ToDto(script);
    }

    public async Task<ScriptDto> CreateAsync(CreateScriptDto dto)
    {
        var script = new Script
        {
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Type = dto.Type,
            Status = dto.Status,
            IsOriginal = dto.IsOriginal,
            IsAdapted = dto.IsAdapted,
            CastMin = dto.CastMin,
            CastMax = dto.CastMax,
            Duration = dto.Duration,
        };

        await _uow.Scripts.AddAsync(script);
        await _uow.SaveChangesAsync();

        return ToDto(script);
    }

    public async Task<ScriptDto?> UpdateAsync(Guid id, UpdateScriptDto dto)
    {
        var script = await _uow.Scripts.GetByIdAsync(id);
        if (script is null) return null;

        if (dto.Title is not null) script.Title = dto.Title;
        if (dto.Subtitle is not null) script.Subtitle = dto.Subtitle;
        if (dto.Type.HasValue) script.Type = dto.Type.Value;
        if (dto.Status.HasValue) script.Status = dto.Status.Value;

        await _uow.Scripts.UpdateAsync(script);
        await _uow.SaveChangesAsync();

        return ToDto(script);
    }

    private static ScriptDto ToDto(Script s) => new()
    {
        Id = s.Id,
        Title = s.Title,
        Subtitle = s.Subtitle,
        Type = s.Type,
        Status = s.Status,
        IsOriginal = s.IsOriginal,
        IsAdapted = s.IsAdapted,
        CastMin = s.CastMin,
        CastMax = s.CastMax,
        Duration = s.Duration,
        LastUsed = s.LastUsed?.ToString("yyyy-MM-dd"),
        ProgramNames = new(),
    };
}
