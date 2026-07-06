using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class PerStarPlanConfiguration : IEntityTypeConfiguration<PerStarPlan>
{
    public void Configure(EntityTypeBuilder<PerStarPlan> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.MonthKey).IsRequired().HasMaxLength(7);
        builder.Property(p => p.MonthlyGoal).HasMaxLength(1000);
        builder.Property(p => p.HowIllSupport).HasMaxLength(1000);
        builder.Property(p => p.Notes).HasMaxLength(1000);
        builder.HasIndex(p => new { p.ParticipantId, p.MonthKey }).IsUnique();

        builder.HasOne(p => p.Participant)
               .WithMany()
               .HasForeignKey(p => p.ParticipantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.AssignedStaff)
               .WithMany()
               .HasForeignKey(p => p.AssignedStaffId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.HasOne(p => p.PriorityObjectiveArea)
               .WithMany()
               .HasForeignKey(p => p.PriorityObjectiveAreaId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.HasOne(p => p.PrioritySubSkill)
               .WithMany()
               .HasForeignKey(p => p.PrioritySubSkillId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
    }
}
