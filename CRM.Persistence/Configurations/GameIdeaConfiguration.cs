using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class GameIdeaConfiguration : IEntityTypeConfiguration<GameIdea>
{
    public void Configure(EntityTypeBuilder<GameIdea> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.Property(i => i.StatusNotes).HasMaxLength(1000);
        builder.Property(i => i.SourceInspiration).HasMaxLength(300);

        builder.HasOne<StaffMember>()
               .WithMany()
               .HasForeignKey(i => i.TeacherSuggestedId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);
    }
}
