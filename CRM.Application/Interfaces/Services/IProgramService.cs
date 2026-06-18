using CRM.Application.DTOs.Programs;

namespace CRM.Application.Interfaces.Services;

public interface IProgramService
{
    Task<IReadOnlyList<ProgramSummaryDto>> GetAllAsync();
    Task<ProgramSummaryDto?> GetBySlugAsync(string slug);
    Task<ProgramDetailDto?> GetDetailAsync(string slug);
    Task<ProgramSummaryDto> CreateAsync(CreateProgramDto dto);

    /// <summary>Updates a program's editable fields by id (slug is immutable). Returns null if not found.</summary>
    Task<ProgramSummaryDto?> UpdateAsync(Guid id, UpdateProgramDto dto);
}
