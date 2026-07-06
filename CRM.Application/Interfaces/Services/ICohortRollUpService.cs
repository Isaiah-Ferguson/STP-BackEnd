using CRM.Application.DTOs.Progress;

namespace CRM.Application.Interfaces.Services;

public interface ICohortRollUpService
{
    /// <summary>Per-skill level counts from confirmed month-end snapshots, optionally scoped to one program.</summary>
    Task<CohortRollUpDto> GetRollUpAsync(string monthKey, Guid? programId);
}
