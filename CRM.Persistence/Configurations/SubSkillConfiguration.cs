using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class SubSkillConfiguration : IEntityTypeConfiguration<SubSkill>
{
    public void Configure(EntityTypeBuilder<SubSkill> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Slug).IsRequired().HasMaxLength(150);
        builder.HasIndex(s => s.Slug).IsUnique();
        builder.HasIndex(s => new { s.ObjectiveAreaId, s.SortOrder });

        // FK to ObjectiveArea configured from the ObjectiveArea side (Restrict).
    }
}
