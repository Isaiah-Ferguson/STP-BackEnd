using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Location).HasMaxLength(200);
        builder.Property(e => e.Meta).HasMaxLength(300);
        builder.Property(e => e.TimeRange).HasMaxLength(50);

        builder.HasOne(e => e.Program)
               .WithMany(p => p.CalendarEvents)
               .HasForeignKey(e => e.ProgramId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);
    }
}
