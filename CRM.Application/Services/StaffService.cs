using CRM.Application.DTOs.Staff;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class StaffService : IStaffService
{
    private readonly IUnitOfWork _uow;

    public StaffService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<StaffSummaryDto>> GetAllAsync()
    {
        var staff = await _uow.Staff.GetAllAsync();
        var programs = await _uow.Programs.GetAllAsync();
        var programMap = programs.ToDictionary(p => p.Id, p => p.Name);

        return staff.Select(s => ToSummary(s, programMap)).ToList();
    }

    public async Task<StaffDetailDto?> GetByIdAsync(Guid id)
    {
        var s = await _uow.Staff.GetByIdAsync(id);
        if (s is null) return null;

        var programs = await _uow.Programs.GetAllAsync();
        var programMap = programs.ToDictionary(p => p.Id, p => p.Name);

        return new StaffDetailDto
        {
            Id = s.Id,
            FullName = s.FullName,
            Initials = s.Initials,
            Role = s.Role,
            StartDate = s.StartDate.ToString("yyyy-MM-dd"),
            OnboardingProgressPct = s.OnboardingProgressPct,
            ProgramNames = new(),
            OnboardingItems = new(),
        };
    }

    public async Task<StaffDetailDto> CreateAsync(CreateStaffDto dto)
    {
        var member = new StaffMember
        {
            FullName = dto.FullName,
            Initials = dto.Initials,
            Role = dto.Role,
            StartDate = dto.StartDate ?? DateTime.UtcNow,
        };

        await _uow.Staff.AddAsync(member);
        await _uow.SaveChangesAsync();

        return (await GetByIdAsync(member.Id))!;
    }

    public async Task<StaffDetailDto?> UpdateAsync(Guid id, UpdateStaffDto dto)
    {
        var member = await _uow.Staff.GetByIdAsync(id);
        if (member is null) return null;

        if (dto.FullName is not null) member.FullName = dto.FullName;
        if (dto.Initials is not null) member.Initials = dto.Initials;
        if (dto.Role.HasValue) member.Role = dto.Role.Value;

        await _uow.Staff.UpdateAsync(member);
        await _uow.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    private static StaffSummaryDto ToSummary(StaffMember s, Dictionary<Guid, string> programMap) =>
        new()
        {
            Id = s.Id,
            FullName = s.FullName,
            Initials = s.Initials,
            Role = s.Role,
            StartDate = s.StartDate.ToString("yyyy-MM-dd"),
            OnboardingProgressPct = s.OnboardingProgressPct,
            ProgramNames = new(),
        };
}
