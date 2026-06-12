using CRM.Application.DTOs.Staff;

namespace CRM.Application.Interfaces.Services;

public interface IStaffService
{
    Task<IReadOnlyList<StaffSummaryDto>> GetAllAsync();
    Task<StaffDetailDto?> GetByIdAsync(Guid id);
    Task<StaffDetailDto> CreateAsync(CreateStaffDto dto);
    Task<StaffDetailDto?> UpdateAsync(Guid id, UpdateStaffDto dto);
}
