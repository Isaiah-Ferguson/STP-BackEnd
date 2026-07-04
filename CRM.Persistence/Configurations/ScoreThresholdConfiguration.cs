using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class ScoreThresholdConfiguration : IEntityTypeConfiguration<ScoreThreshold>
{
    public void Configure(EntityTypeBuilder<ScoreThreshold> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Level).IsUnique();
    }
}
