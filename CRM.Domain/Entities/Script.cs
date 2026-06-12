using CRM.Domain.Common;
using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Script : BaseEntity
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
    public DateTime? LastUsed { get; set; }

    public ICollection<ScriptProgram> ScriptPrograms { get; set; } = new List<ScriptProgram>();
}
