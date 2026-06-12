namespace CRM.Domain.Entities;

public class StaffProgramAssignment
{
    public Guid StaffMemberId { get; set; }
    public Guid ProgramId { get; set; }

    public StaffMember StaffMember { get; set; } = null!;
    public CrmProgram Program { get; set; } = null!;
}
