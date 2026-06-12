using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class StaffProgramAssignmentConfiguration : IEntityTypeConfiguration<StaffProgramAssignment>
{
    public void Configure(EntityTypeBuilder<StaffProgramAssignment> builder)
    {
        builder.HasKey(a => new { a.StaffMemberId, a.ProgramId });

        builder.HasOne(a => a.StaffMember)
               .WithMany(s => s.ProgramAssignments)
               .HasForeignKey(a => a.StaffMemberId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Program)
               .WithMany(p => p.StaffAssignments)
               .HasForeignKey(a => a.ProgramId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
