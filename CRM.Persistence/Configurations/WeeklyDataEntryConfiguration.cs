using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class WeeklyDataEntryConfiguration : IEntityTypeConfiguration<WeeklyDataEntry>
{
    public void Configure(EntityTypeBuilder<WeeklyDataEntry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.MonthKey).IsRequired().HasMaxLength(7);
        builder.HasIndex(e => new { e.ParticipantId, e.SubSkillId, e.MonthKey, e.WeekNumber });
        builder.HasIndex(e => new { e.ParticipantId, e.MonthKey });

        builder.HasOne(e => e.Participant)
               .WithMany()
               .HasForeignKey(e => e.ParticipantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.SubSkill)
               .WithMany()
               .HasForeignKey(e => e.SubSkillId)
               .OnDelete(DeleteBehavior.Restrict);

        // Session bridge — nullable, detaches (does not delete data) if the session goes away.
        builder.HasOne<Session>()
               .WithMany()
               .HasForeignKey(e => e.SessionId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        // Audit stamp — recorder link without a navigation property.
        builder.HasOne<StaffMember>()
               .WithMany()
               .HasForeignKey(e => e.RecordedByStaffMemberId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);
    }
}
