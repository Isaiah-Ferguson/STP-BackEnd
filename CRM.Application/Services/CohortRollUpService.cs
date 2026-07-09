using CRM.Application.DTOs.Progress;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class CohortRollUpService : ICohortRollUpService
{
    private readonly IUnitOfWork _uow;

    public CohortRollUpService(IUnitOfWork uow) => _uow = uow;

    public async Task<CohortRollUpDto> GetRollUpAsync(string monthKey, Guid? programId)
    {
        // Only confirmed snapshots count toward the roll-up. When scoped to a program,
        // resolve the participant ids first so the snapshot filter runs in SQL
        // (Contains translates to IN) instead of loading the whole month and filtering
        // in memory (#29).
        string? programName = null;
        IReadOnlyList<Domain.Entities.MonthlyProgressSnapshot> snaps;
        if (programId is { } pid)
        {
            var program = await _uow.Programs.GetByIdAsync(pid);
            programName = program?.Name;
            var inProgram = (await _uow.Participants.ListAsync(p => p.ProgramId == pid))
                .Select(p => p.Id)
                .ToHashSet();
            snaps = await _uow.MonthlyProgressSnapshots.ListAsync(
                s => s.MonthKey == monthKey && s.IsConfirmed && inProgram.Contains(s.ParticipantId));
        }
        else
        {
            snaps = await _uow.MonthlyProgressSnapshots.ListAsync(
                s => s.MonthKey == monthKey && s.IsConfirmed);
        }

        var subSkills = (await _uow.SubSkills.GetAllAsync()).Where(s => s.IsActive).ToList();
        var areas = (await _uow.ObjectiveAreas.GetAllAsync()).ToDictionary(a => a.Id);
        var bySkill = snaps.GroupBy(s => s.SubSkillId).ToDictionary(g => g.Key, g => g.ToList());

        var rows = subSkills
            .OrderBy(s => s.SectionNumber).ThenBy(s => s.SortOrder)
            .Select(skill =>
            {
                var list = bySkill.GetValueOrDefault(skill.Id) ?? new();
                var nov = list.Count(s => s.Level == ProgressLevel.Novice);
                var inter = list.Count(s => s.Level == ProgressLevel.Intermediate);
                var exp = list.Count(s => s.Level == ProgressLevel.Expert);
                var na = list.Count(s => s.Level == ProgressLevel.NotApplicable);
                var area = areas.GetValueOrDefault(skill.ObjectiveAreaId);

                return new CohortRollUpRowDto
                {
                    SubSkillId = skill.Id,
                    SubSkillName = skill.Name,
                    SectionNumber = skill.SectionNumber,
                    ObjectiveAreaName = area?.Name ?? "",
                    ObjectiveAreaColorHex = area?.ColorHex ?? "",
                    NoviceCount = nov,
                    IntermediateCount = inter,
                    ExpertCount = exp,
                    NotApplicableCount = na,
                    ScoredCount = nov + inter + exp,
                    MostCommonLevel = MostCommon(nov, inter, exp),
                };
            })
            .ToList();

        return new CohortRollUpDto
        {
            MonthKey = monthKey,
            ProgramId = programId,
            ProgramName = programName,
            ParticipantCount = snaps.Select(s => s.ParticipantId).Distinct().Count(),
            Rows = rows,
        };
    }

    // Mode of the three real levels; ties resolve to the higher level. "—" when nothing scored.
    private static string MostCommon(int nov, int inter, int exp)
    {
        if (nov + inter + exp == 0) return "—";
        if (exp >= inter && exp >= nov) return "Expert";
        if (inter >= nov) return "Intermediate";
        return "Novice";
    }
}
