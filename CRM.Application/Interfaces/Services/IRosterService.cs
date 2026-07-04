using CRM.Application.DTOs.Roster;

namespace CRM.Application.Interfaces.Services;

public interface IRosterService
{
    /// <summary>Every participant's roster row for a term (management view). Optionally filtered to one site.</summary>
    Task<IReadOnlyList<RosterEntryDto>> GetRosterAsync(int year, int quarter, Guid? siteId);

    /// <summary>The caller's in-scope participants for a term (their assigned programs; all for admins).</summary>
    Task<IReadOnlyList<RosterEntryDto>> GetMyStarsAsync(Guid userId, int year, int quarter);

    /// <summary>Creates or updates a participant's assignment for a term.</summary>
    Task<RosterEntryDto> UpsertAssignmentAsync(UpsertRosterAssignmentDto dto);
}
