using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class OnboardingItemConfiguration : IEntityTypeConfiguration<OnboardingItem>
{
    public void Configure(EntityTypeBuilder<OnboardingItem> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Section).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Label).IsRequired().HasMaxLength(300);

        builder.HasOne(o => o.StaffMember)
               .WithMany(s => s.OnboardingItems)
               .HasForeignKey(o => o.StaffMemberId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
