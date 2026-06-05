using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application;

/// <summary>
/// Extension method to register all Application layer services.
/// Called from Program.cs: builder.Services.AddApplicationServices()
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // TODO: Register application services, validators, AutoMapper profiles, etc.
        // Example: services.AddScoped<IContactService, ContactService>();

        return services;
    }
}
