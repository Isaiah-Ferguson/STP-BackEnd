using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class CrmProgramConfiguration : IEntityTypeConfiguration<CrmProgram>
{
    public void Configure(EntityTypeBuilder<CrmProgram> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Slug).IsRequired().HasMaxLength(100);
        builder.Property(p => p.ColorHex).IsRequired().HasMaxLength(7);
        builder.Property(p => p.DefaultLocation).HasMaxLength(200);
        builder.Property(p => p.SessionSchedule).HasMaxLength(200);
        builder.HasIndex(p => p.Slug).IsUnique();
    }
}
