using CRM.Application.DTOs.Dashboard;

namespace CRM.Application.Interfaces.Services;

public interface IDashboardService
{
    /// <summary>Composes the full dashboard payload in a single call.</summary>
    Task<DashboardDto> GetAsync();
}
