using CRM.Application.Interfaces.Services;
using CRM.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITaxonomyService, TaxonomyService>();
        services.AddScoped<IProgramService, ProgramService>();
        services.AddScoped<IParticipantService, ParticipantService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IScriptService, ScriptService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportsService, ReportsService>();

        return services;
    }
}
