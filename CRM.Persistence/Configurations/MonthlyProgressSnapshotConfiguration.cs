using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class MonthlyProgressSnapshotConfiguration : IEntityTypeConfiguration<MonthlyProgressSnapshot>
{
    public void Configure(EntityTypeBuilder<MonthlyProgressSnapshot> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.MonthKey).IsRequired().HasMaxLength(7);

        // One snapshot per participant per skill per month.
        builder.HasIndex(s => new { s.ParticipantId, s.SubSkillId, s.MonthKey }).IsUnique();
        builder.HasIndex(s => new { s.MonthKey, s.SubSkillId }); // roll-up query

        builder.HasOne(s => s.Participant)
               .WithMany()
               .HasForeignKey(s => s.ParticipantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.SubSkill)
               .WithMany()
               .HasForeignKey(s => s.SubSkillId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StaffMember>()
               .WithMany()
               .HasForeignKey(s => s.ConfirmedByStaffMemberId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);
    }
}
