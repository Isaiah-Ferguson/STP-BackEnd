using Microsoft.EntityFrameworkCore;

namespace CRM.Persistence;

/// <summary>
/// The main EF Core database context for the CRM application.
/// 
/// This context connects to Azure SQL (SQL Server) via the connection string
/// defined in appsettings.json under "ConnectionStrings:DefaultConnection".
/// 
/// To add a new entity later:
///   1. Create the entity class in CRM.Domain/Entities/
///   2. Add a DbSet here: public DbSet<MyEntity> MyEntities { get; set; }
///   3. Run: dotnet ef migrations add <MigrationName> --project CRM.Persistence --startup-project CRM.API
///   4. Run: dotnet ef database update --project CRM.Persistence --startup-project CRM.API
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // TODO: Add DbSet properties here as entities are created.
    // Example:
    // public DbSet<Contact> Contacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TODO: Apply entity configurations here.
        // Example: modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
