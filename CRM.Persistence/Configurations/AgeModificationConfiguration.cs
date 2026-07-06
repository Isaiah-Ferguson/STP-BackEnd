using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class AgeModificationConfiguration : IEntityTypeConfiguration<AgeModification>
{
    public void Configure(EntityTypeBuilder<AgeModification> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.GameName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.GroupAgeLevel).HasMaxLength(150);
        builder.Property(m => m.Modification).IsRequired().HasMaxLength(1000);

        builder.HasOne<StaffMember>()
               .WithMany()
               .HasForeignKey(m => m.TeacherSuggestedId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        builder.HasOne<Game>()
               .WithMany()
               .HasForeignKey(m => m.GameId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);
    }
}
