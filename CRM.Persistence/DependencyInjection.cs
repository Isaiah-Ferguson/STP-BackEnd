using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Persistence;

/// <summary>
/// Extension method to register all Persistence layer services.
/// Called from Program.cs: builder.Services.AddPersistenceServices(config)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register EF Core with SQL Server (Azure SQL)
        // Connection string is read from appsettings.json → "ConnectionStrings:DefaultConnection"
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            ));

        // TODO: Register repository implementations here.
        // Example: services.AddScoped<IContactRepository, ContactRepository>();

        return services;
    }
}
