using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class WeeklyFocusSkillConfiguration : IEntityTypeConfiguration<WeeklyFocusSkill>
{
    public void Configure(EntityTypeBuilder<WeeklyFocusSkill> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.MonthKey).IsRequired().HasMaxLength(7);
        builder.HasIndex(f => new { f.ProgramId, f.MonthKey, f.WeekNumber, f.SubSkillId }).IsUnique();

        builder.HasOne(f => f.Program)
               .WithMany()
               .HasForeignKey(f => f.ProgramId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.SubSkill)
               .WithMany()
               .HasForeignKey(f => f.SubSkillId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
