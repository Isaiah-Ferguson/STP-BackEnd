using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class AttendanceNoteConfiguration : IEntityTypeConfiguration<AttendanceNote>
{
    public void Configure(EntityTypeBuilder<AttendanceNote> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Content).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.NoteType).IsRequired().HasMaxLength(20);

        builder.HasOne(n => n.AttendanceRecord)
               .WithMany(r => r.Notes)
               .HasForeignKey(n => n.AttendanceRecordId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
