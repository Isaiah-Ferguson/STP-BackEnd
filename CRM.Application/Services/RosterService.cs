using CRM.Application.DTOs.Roster;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class RosterService : IRosterService
{
    private readonly IUnitOfWork _uow;

    public RosterService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<RosterEntryDto>> GetRosterAsync(int year, int quarter, Guid? siteId)
    {
        var participants = await _uow.Participants.GetAllAsync();
        var ctx = await LoadContextAsync(year, quarter);

        var entries = participants
            .Select(p => BuildEntry(p, ctx.AssignmentByParticipant.GetValueOrDefault(p.Id), year, quarter, ctx))
            .Where(e => siteId is null || e.SiteId == siteId);

        return Order(entries).ToList();
    }

    public async Task<IReadOnlyList<RosterEntryDto>> GetMyStarsAsync(Guid userId, int year, int quarter)
    {
        var participants = await _uow.Participants.GetAllAsync();
        var ctx = await LoadContextAsync(year, quarter);
        var allowed = await AllowedProgramIdsAsync(userId, ctx.Programs.Keys);

        var entries = participants
            .Where(p => allowed.Contains(p.ProgramId))
            .Select(p => BuildEntry(p, ctx.AssignmentByParticipant.GetValueOrDefault(p.Id), year, quarter, ctx));

        return Order(entries).ToList();
    }

    public async Task<RosterEntryDto> UpsertAssignmentAsync(UpsertRosterAssignmentDto dto)
    {
        var existing = (await _uow.RosterAssignments.ListAsync(
            r => r.ParticipantId == dto.ParticipantId && r.Year == dto.Year && r.Quarter == dto.Quarter)).FirstOrDefault();

        if (existing is null)
        {
            existing = new RosterAssignment
            {
                ParticipantId = dto.ParticipantId,
                Year = dto.Year,
                Quarter = dto.Quarter,
                SiteId = dto.SiteId,
                StarGroupId = dto.StarGroupId,
                AssignedStaffId = dto.AssignedStaffId,
                CountedInRatio = dto.CountedInRatio,
                Notes = dto.Notes,
            };
            await _uow.RosterAssignments.AddAsync(existing);
        }
        else
        {
            existing.SiteId = dto.SiteId;
            existing.StarGroupId = dto.StarGroupId;
            existing.AssignedStaffId = dto.AssignedStaffId;
            existing.CountedInRatio = dto.CountedInRatio;
            existing.Notes = dto.Notes;
            await _uow.RosterAssignments.UpdateAsync(existing);
        }

        await _uow.SaveChangesAsync();

        var ctx = await LoadContextAsync(dto.Year, dto.Quarter);
        var participant = await _uow.Participants.GetByIdAsync(dto.ParticipantId)
                          ?? throw new ArgumentException($"Participant {dto.ParticipantId} not found.");
        return BuildEntry(participant, existing, dto.Year, dto.Quarter, ctx);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private sealed record Ctx(
        Dictionary<Guid, CrmProgram> Programs,
        Dictionary<Guid, Site> Sites,
        Dictionary<Guid, StarGroup> Groups,
        Dictionary<Guid, StaffMember> Staff,
        Dictionary<Guid, RosterAssignment> AssignmentByParticipant);

    private async Task<Ctx> LoadContextAsync(int year, int quarter)
    {
        var programs = await _uow.Programs.GetAllAsync();
        var sites = await _uow.Sites.GetAllAsync();
        var groups = await _uow.StarGroups.GetAllAsync();
        var staff = await _uow.Staff.GetAllAsync();
        var assignments = await _uow.RosterAssignments.ListAsync(r => r.Year == year && r.Quarter == quarter);
        return new Ctx(
            programs.ToDictionary(p => p.Id),
            sites.ToDictionary(s => s.Id),
            groups.ToDictionary(g => g.Id),
            staff.ToDictionary(s => s.Id),
            assignments.ToDictionary(a => a.ParticipantId));
    }

    private static RosterEntryDto BuildEntry(Participant p, RosterAssignment? a, int year, int quarter, Ctx ctx)
    {
        var program = ctx.Programs.GetValueOrDefault(p.ProgramId);
        var dto = new RosterEntryDto
        {
            ParticipantId = p.Id,
            ParticipantName = p.FullName,
            ParticipantInitials = p.Initials,
            ProgramId = p.ProgramId,
            ProgramName = program?.Name ?? "",
            ProgramSlug = program?.Slug ?? "",
            Quarter = quarter,
            Year = year,
        };

        if (a is not null)
        {
            dto.AssignmentId = a.Id;
            dto.SiteId = a.SiteId;
            dto.SiteName = a.SiteId is { } sid ? ctx.Sites.GetValueOrDefault(sid)?.Name : null;
            dto.StarGroupId = a.StarGroupId;
            dto.StarGroupName = a.StarGroupId is { } gid ? ctx.Groups.GetValueOrDefault(gid)?.Name : null;
            dto.AssignedStaffId = a.AssignedStaffId;
            dto.AssignedStaffName = a.AssignedStaffId is { } stid ? ctx.Staff.GetValueOrDefault(stid)?.FullName : null;
            dto.CountedInRatio = a.CountedInRatio;
            dto.Notes = a.Notes;
        }
        return dto;
    }

    private static IEnumerable<RosterEntryDto> Order(IEnumerable<RosterEntryDto> entries) =>
        entries.OrderBy(e => e.SiteName ?? "~")   // unassigned (null) sorts last
               .ThenBy(e => e.StarGroupName ?? "~")
               .ThenBy(e => e.ParticipantName);

    private async Task<HashSet<Guid>> AllowedProgramIdsAsync(Guid userId, IEnumerable<Guid> allProgramIds)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user?.Role == UserRole.Admin) return allProgramIds.ToHashSet();
        if (user?.StaffMemberId is not { } staffId) return new HashSet<Guid>();

        var assignments = await _uow.GetStaffProgramAssignmentsAsync();
        return assignments
            .Where(a => a.StaffMemberId == staffId)
            .Select(a => a.ProgramId)
            .ToHashSet();
    }
}
