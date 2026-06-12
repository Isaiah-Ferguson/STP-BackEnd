using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class DocumentRecordConfiguration : IEntityTypeConfiguration<DocumentRecord>
{
    public void Configure(EntityTypeBuilder<DocumentRecord> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.DocumentType).IsRequired().HasMaxLength(100);

        builder.HasOne(d => d.Participant)
               .WithMany(p => p.Documents)
               .HasForeignKey(d => d.ParticipantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
