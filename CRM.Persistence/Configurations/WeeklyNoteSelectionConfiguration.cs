using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class WeeklyNoteSelectionConfiguration : IEntityTypeConfiguration<WeeklyNoteSelection>
{
    public void Configure(EntityTypeBuilder<WeeklyNoteSelection> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.MonthKey).IsRequired().HasMaxLength(7);
        builder.Property(n => n.CustomText).HasMaxLength(1000);
        builder.HasIndex(n => new { n.ParticipantId, n.MonthKey, n.WeekNumber, n.Kind }).IsUnique();

        builder.HasOne(n => n.Participant)
               .WithMany()
               .HasForeignKey(n => n.ParticipantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.GoalBankEntry)
               .WithMany()
               .HasForeignKey(n => n.GoalBankEntryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
    }
}
