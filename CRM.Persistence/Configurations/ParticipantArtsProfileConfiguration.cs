using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Configurations;

public class ParticipantArtsProfileConfiguration : IEntityTypeConfiguration<ParticipantArtsProfile>
{
    public void Configure(EntityTypeBuilder<ParticipantArtsProfile> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.IppSummary).HasMaxLength(2000);
        builder.Property(a => a.CurrentLevel).HasMaxLength(2000);
        builder.Property(a => a.TsspArtsGoal).HasMaxLength(2000);

        // One profile per participant.
        builder.HasIndex(a => a.ParticipantId).IsUnique();

        builder.HasOne(a => a.Participant)
               .WithOne()
               .HasForeignKey<ParticipantArtsProfile>(a => a.ParticipantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
