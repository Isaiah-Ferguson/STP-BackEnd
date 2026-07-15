using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class ChecklistTemplateItemConfiguration : IEntityTypeConfiguration<ChecklistTemplateItem>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplateItem> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Section).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Label).IsRequired().HasMaxLength(300);
    }
}
