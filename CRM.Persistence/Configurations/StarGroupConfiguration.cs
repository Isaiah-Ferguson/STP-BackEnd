using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class StarGroupConfiguration : IEntityTypeConfiguration<StarGroup>
{
    public void Configure(EntityTypeBuilder<StarGroup> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired().HasMaxLength(150);
        builder.Property(g => g.Slug).IsRequired().HasMaxLength(100);
        builder.HasIndex(g => g.Slug).IsUnique();
    }
}
