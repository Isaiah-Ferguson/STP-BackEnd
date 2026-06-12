using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Scripts;

public class ScriptDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public ScriptType Type { get; set; }
    public ScriptStatus Status { get; set; }
    public bool IsOriginal { get; set; }
    public bool IsAdapted { get; set; }
    public int? CastMin { get; set; }
    public int? CastMax { get; set; }
    public string? Duration { get; set; }
    public string? LastUsed { get; set; }
    public List<string> ProgramNames { get; set; } = new();
}

public class CreateScriptDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public ScriptType Type { get; set; } = ScriptType.Play;
    public ScriptStatus Status { get; set; } = ScriptStatus.Draft;
    public bool IsOriginal { get; set; }
    public bool IsAdapted { get; set; }
    public int? CastMin { get; set; }
    public int? CastMax { get; set; }
    public string? Duration { get; set; }
    public List<Guid> ProgramIds { get; set; } = new();
}

public class UpdateScriptDto
{
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public ScriptType? Type { get; set; }
    public ScriptStatus? Status { get; set; }
    public List<Guid>? ProgramIds { get; set; }
}
