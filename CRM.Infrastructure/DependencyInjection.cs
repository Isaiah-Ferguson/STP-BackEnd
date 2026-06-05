using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Infrastructure;

/// <summary>
/// Extension method to register all Infrastructure layer services.
/// Called from Program.cs: builder.Services.AddInfrastructureServices(config)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // TODO: Register external service clients here.
        // Examples:
        //   - Azure Blob Storage client
        //   - Email service (SendGrid, SMTP, etc.)
        //   - Third-party API clients

        return services;
    }
}
