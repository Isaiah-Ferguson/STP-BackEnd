namespace CRM.Domain.Entities;

public class ScriptProgram
{
    public Guid ScriptId { get; set; }
    public Guid ProgramId { get; set; }

    public Script Script { get; set; } = null!;
    public CrmProgram Program { get; set; } = null!;
}
