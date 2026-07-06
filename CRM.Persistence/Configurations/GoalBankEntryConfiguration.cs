using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class GoalBankEntryConfiguration : IEntityTypeConfiguration<GoalBankEntry>
{
    public void Configure(EntityTypeBuilder<GoalBankEntry> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Text).IsRequired().HasMaxLength(1000);
        builder.HasIndex(g => new { g.Kind, g.SectionNumber, g.Level });
    }
}
