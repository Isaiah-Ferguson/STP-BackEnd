using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class GameSubGoalConfiguration : IEntityTypeConfiguration<GameSubGoal>
{
    public void Configure(EntityTypeBuilder<GameSubGoal> builder)
    {
        builder.HasKey(sg => sg.Id);
        builder.HasIndex(sg => new { sg.GameId, sg.SubSkillId }).IsUnique();

        // Game side of the relationship is configured from GameConfiguration (Cascade).
        builder.HasOne(sg => sg.SubSkill)
               .WithMany()
               .HasForeignKey(sg => sg.SubSkillId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
