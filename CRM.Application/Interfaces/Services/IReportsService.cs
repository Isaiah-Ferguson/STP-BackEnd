using CRM.Application.DTOs.Reports;

namespace CRM.Application.Interfaces.Services;

public interface IReportsService
{
    /// <summary>Computes the org-wide reporting snapshot in a single call.</summary>
    Task<ReportsDto> GetAsync(CancellationToken ct = default);
}
