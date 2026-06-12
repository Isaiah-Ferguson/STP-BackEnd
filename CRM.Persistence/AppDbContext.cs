using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CrmProgram> Programs { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<StaffMember> Staff { get; set; }
    public DbSet<StaffProgramAssignment> StaffProgramAssignments { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<AttendanceNote> AttendanceNotes { get; set; }
    public DbSet<CalendarEvent> CalendarEvents { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<Script> Scripts { get; set; }
    public DbSet<ScriptProgram> ScriptPrograms { get; set; }
    public DbSet<DocumentRecord> DocumentRecords { get; set; }
    public DbSet<OnboardingItem> OnboardingItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
