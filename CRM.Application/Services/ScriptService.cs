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
        var namesByScript = await BuildProgramNameMapAsync();
        return scripts.Select(s => ToDto(s, namesByScript.GetValueOrDefault(s.Id))).ToList();
    }

    public async Task<ScriptDto?> GetByIdAsync(Guid id)
    {
        var script = await _uow.Scripts.GetByIdAsync(id);
        if (script is null) return null;
        var namesByScript = await BuildProgramNameMapAsync();
        return ToDto(script, namesByScript.GetValueOrDefault(id));
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

        await _uow.ReplaceScriptProgramsAsync(script.Id, dto.ProgramIds);
        await _uow.SaveChangesAsync();

        return await GetByIdAsync(script.Id) ?? ToDto(script, null);
    }

    public async Task<ScriptDto?> UpdateAsync(Guid id, UpdateScriptDto dto)
    {
        var script = await _uow.Scripts.GetByIdAsync(id);
        if (script is null) return null;

        // PUT is a full edit from the Script Library form: apply every provided field.
        if (dto.Title is not null) script.Title = dto.Title;
        script.Subtitle = dto.Subtitle;
        if (dto.Type.HasValue) script.Type = dto.Type.Value;
        if (dto.Status.HasValue) script.Status = dto.Status.Value;
        if (dto.IsOriginal.HasValue) script.IsOriginal = dto.IsOriginal.Value;
        if (dto.IsAdapted.HasValue) script.IsAdapted = dto.IsAdapted.Value;
        script.CastMin = dto.CastMin;
        script.CastMax = dto.CastMax;
        script.Duration = dto.Duration;

        await _uow.Scripts.UpdateAsync(script);

        if (dto.ProgramIds is not null)
            await _uow.ReplaceScriptProgramsAsync(id, dto.ProgramIds);

        await _uow.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    /// <summary>scriptId → the display names of the programs it's linked to.</summary>
    private async Task<Dictionary<Guid, List<string>>> BuildProgramNameMapAsync()
    {
        var links = await _uow.GetScriptProgramsAsync();
        if (links.Count == 0) return new();

        var programs = await _uow.Programs.GetAllAsync();
        var nameById = programs.ToDictionary(p => p.Id, p => p.Name);

        return links
            .GroupBy(l => l.ScriptId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(l => nameById.GetValueOrDefault(l.ProgramId))
                      .Where(n => n is not null)
                      .Select(n => n!)
                      .ToList());
    }

    private static ScriptDto ToDto(Script s, List<string>? programNames) => new()
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
        ProgramNames = programNames ?? new(),
    };
}
