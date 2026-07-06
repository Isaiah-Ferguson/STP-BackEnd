using CRM.Application.DTOs.Planning;

namespace CRM.Application.Interfaces.Services;

public interface IPlanningService
{
    /// <summary>Every participant's per-Star plan row for a month (optionally scoped to one program).</summary>
    Task<IReadOnlyList<PerStarPlanDto>> GetPerStarPlansAsync(string monthKey, Guid? programId);

    /// <summary>Creates or updates a participant's plan for a month.</summary>
    Task<PerStarPlanDto> UpsertPerStarPlanAsync(UpsertPerStarPlanDto dto);
}
