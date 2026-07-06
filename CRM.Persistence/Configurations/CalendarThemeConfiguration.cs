using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class CalendarThemeConfiguration : IEntityTypeConfiguration<CalendarTheme>
{
    public void Configure(EntityTypeBuilder<CalendarTheme> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.ThemeTitle).IsRequired().HasMaxLength(150);
        builder.Property(t => t.ThemeSubtitle).HasMaxLength(300);
        builder.Property(t => t.KeyArtsDatesText).HasMaxLength(500);
        builder.Property(t => t.FeaturedGamesText).HasMaxLength(1000);
        builder.Property(t => t.AlternativeOptionsText).HasMaxLength(1000);
        builder.Property(t => t.ProductionPhase).HasMaxLength(300);
        builder.Property(t => t.ProgrammingNotes).HasMaxLength(1000);
        builder.HasIndex(t => t.Month).IsUnique();
    }
}
