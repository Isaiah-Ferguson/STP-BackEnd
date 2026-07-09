using CRM.Application.DTOs.Participants;

namespace CRM.Application.Interfaces.Services;

public interface IParticipantService
{
    Task<IReadOnlyList<ParticipantSummaryDto>> GetAllAsync(CancellationToken ct = default);
    Task<ParticipantDetailDto?> GetByIdAsync(Guid id);
    Task<ParticipantDetailDto> CreateAsync(CreateParticipantDto dto);
    Task<ParticipantDetailDto?> UpdateAsync(Guid id, UpdateParticipantDto dto);
    Task<bool> DeleteAsync(Guid id);
}
