using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class MonthlySummaryConfiguration : IEntityTypeConfiguration<MonthlySummary>
{
    public void Configure(EntityTypeBuilder<MonthlySummary> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.MonthKey).IsRequired().HasMaxLength(7);
        builder.Property(s => s.ProgressNarrative).HasMaxLength(2000);
        builder.Property(s => s.NextMonthUpdate).HasMaxLength(2000);
        builder.HasIndex(s => new { s.ParticipantId, s.MonthKey }).IsUnique();

        builder.HasOne(s => s.Participant)
               .WithMany()
               .HasForeignKey(s => s.ParticipantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
