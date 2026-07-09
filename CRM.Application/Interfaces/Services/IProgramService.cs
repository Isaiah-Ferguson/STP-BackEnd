using CRM.Application.DTOs.Programs;

namespace CRM.Application.Interfaces.Services;

public interface IProgramService
{
    Task<IReadOnlyList<ProgramSummaryDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>The programs the caller may work in: all for an Admin, otherwise their linked staff's assigned programs.</summary>
    Task<IReadOnlyList<ProgramSummaryDto>> GetForUserAsync(Guid userId, CancellationToken ct = default);

    Task<ProgramSummaryDto?> GetBySlugAsync(string slug);
    Task<ProgramDetailDto?> GetDetailAsync(string slug);
    Task<ProgramSummaryDto> CreateAsync(CreateProgramDto dto);

    /// <summary>Updates a program's editable fields by id (slug is immutable). Returns null if not found.</summary>
    Task<ProgramSummaryDto?> UpdateAsync(Guid id, UpdateProgramDto dto);
}
