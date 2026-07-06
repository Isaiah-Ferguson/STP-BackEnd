using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class KeyArtsDateConfiguration : IEntityTypeConfiguration<KeyArtsDate>
{
    public void Configure(EntityTypeBuilder<KeyArtsDate> builder)
    {
        builder.HasKey(k => k.Id);
        builder.Property(k => k.DateText).HasMaxLength(50);
        builder.Property(k => k.Observance).IsRequired().HasMaxLength(200);
        builder.Property(k => k.ObservanceType).HasMaxLength(50);
        builder.Property(k => k.ProgrammingTieIn).HasMaxLength(300);
        builder.HasIndex(k => new { k.Month, k.SortOrder });
    }
}
