using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Room).HasMaxLength(100);
        builder.Property(s => s.TimeRange).HasMaxLength(50);
        builder.Property(s => s.Label).HasMaxLength(200);

        builder.HasOne(s => s.Program)
               .WithMany(p => p.Sessions)
               .HasForeignKey(s => s.ProgramId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
