using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Initials).IsRequired().HasMaxLength(5);
        builder.Property(p => p.ServiceCoordinator).HasMaxLength(200);

        builder.HasOne(p => p.Program)
               .WithMany(pr => pr.Participants)
               .HasForeignKey(p => p.ProgramId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
