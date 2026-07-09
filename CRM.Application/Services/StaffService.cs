using CRM.Application.DTOs.Staff;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class StaffService : IStaffService
{
    private readonly IUnitOfWork _uow;

    public StaffService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<StaffSummaryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var staff = await _uow.Staff.GetAllAsync(ct);
        var programs = await _uow.Programs.GetAllAsync(ct);
        var assignments = await _uow.GetStaffProgramAssignmentsAsync();

        var programMap = programs.ToDictionary(p => p.Id, p => p.Name);
        var assignmentsByStaff = assignments
            .GroupBy(a => a.StaffMemberId)
            .ToDictionary(g => g.Key, g => g.Select(a => a.ProgramId).ToList());

        return staff.Select(s =>
        {
            var progIds = assignmentsByStaff.GetValueOrDefault(s.Id, new());
            var progNames = progIds
                .Select(id => programMap.GetValueOrDefault(id))
                .OfType<string>()
                .ToList();
            return ToSummary(s, progNames);
        }).ToList();
    }

    public async Task<StaffDetailDto?> GetByIdAsync(Guid id)
    {
        var s = await _uow.Staff.GetByIdAsync(id);
        if (s is null) return null;

        var programs = await _uow.Programs.GetAllAsync();
        var assignments = await _uow.GetStaffProgramAssignmentsAsync();
        var onboardingItems = await _uow.OnboardingItems.ListAsync(o => o.StaffMemberId == id);

        var programMap = programs.ToDictionary(p => p.Id, p => p.Name);
        var progNames = assignments
            .Where(a => a.StaffMemberId == id)
            .Select(a => programMap.GetValueOrDefault(a.ProgramId))
            .OfType<string>()
            .ToList();

        return new StaffDetailDto
        {
            Id = s.Id,
            FullName = s.FullName,
            Initials = s.Initials,
            Role = s.Role,
            StartDate = s.StartDate.ToString("yyyy-MM-dd"),
            OnboardingProgressPct = s.OnboardingProgressPct,
            ProgramNames = progNames,
            OnboardingItems = onboardingItems.Select(o => new OnboardingItemDto
            {
                Id = o.Id,
                Section = o.Section,
                Label = o.Label,
                IsCompleted = o.IsCompleted,
                CompletedDate = o.CompletedDate?.ToString("yyyy-MM-dd"),
                ExpiryDate = o.ExpiryDate?.ToString("yyyy-MM-dd"),
            }).ToList(),
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

        if (dto.ProgramIds is { Count: > 0 })
        {
            foreach (var progId in dto.ProgramIds)
                await _uow.AddStaffProgramAssignmentAsync(new StaffProgramAssignment
                {
                    StaffMemberId = member.Id,
                    ProgramId = progId,
                });
            await _uow.SaveChangesAsync();
        }

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

    private static StaffSummaryDto ToSummary(StaffMember s, List<string> programNames) =>
        new()
        {
            Id = s.Id,
            FullName = s.FullName,
            Initials = s.Initials,
            Role = s.Role,
            StartDate = s.StartDate.ToString("yyyy-MM-dd"),
            OnboardingProgressPct = s.OnboardingProgressPct,
            ProgramNames = programNames,
        };
}
