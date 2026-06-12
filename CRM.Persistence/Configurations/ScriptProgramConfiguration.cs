using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class ScriptProgramConfiguration : IEntityTypeConfiguration<ScriptProgram>
{
    public void Configure(EntityTypeBuilder<ScriptProgram> builder)
    {
        builder.HasKey(sp => new { sp.ScriptId, sp.ProgramId });

        builder.HasOne(sp => sp.Script)
               .WithMany(s => s.ScriptPrograms)
               .HasForeignKey(sp => sp.ScriptId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Program)
               .WithMany(p => p.ScriptPrograms)
               .HasForeignKey(sp => sp.ProgramId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
