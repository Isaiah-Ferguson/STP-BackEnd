using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Group).HasMaxLength(100);

        builder.HasOne(r => r.Participant)
               .WithMany(p => p.AttendanceRecords)
               .HasForeignKey(r => r.ParticipantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Session)
               .WithMany(s => s.AttendanceRecords)
               .HasForeignKey(r => r.SessionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
