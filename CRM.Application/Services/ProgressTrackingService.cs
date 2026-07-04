using System.Globalization;
using CRM.Application.DTOs.Progress;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class ProgressTrackingService : IProgressTrackingService
{
    private readonly IUnitOfWork _uow;

    public ProgressTrackingService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<WeeklyFocusSkillDto>> GetFocusSkillsAsync(Guid programId, string monthKey)
    {
        var focus = await _uow.WeeklyFocusSkills.ListAsync(f => f.ProgramId == programId && f.MonthKey == monthKey);
        var skills = await SubSkillMapAsync();
        return focus
            .OrderBy(f => f.WeekNumber)
            .ThenBy(f => skills.GetValueOrDefault(f.SubSkillId)?.SectionNumber ?? 0)
            .Select(f => ToFocusDto(f, skills))
            .ToList();
    }

    public async Task<IReadOnlyList<WeeklyFocusSkillDto>> SetFocusSkillsAsync(SetFocusSkillsDto dto)
    {
        var existing = await _uow.WeeklyFocusSkills.ListAsync(
            f => f.ProgramId == dto.ProgramId && f.MonthKey == dto.MonthKey && f.WeekNumber == dto.WeekNumber);
        foreach (var old in existing)
            await _uow.WeeklyFocusSkills.DeleteAsync(old);

        foreach (var subId in dto.SubSkillIds.Distinct())
            await _uow.WeeklyFocusSkills.AddAsync(new WeeklyFocusSkill
            {
                ProgramId = dto.ProgramId,
                MonthKey = dto.MonthKey,
                WeekNumber = dto.WeekNumber,
                SubSkillId = subId,
            });

        await _uow.SaveChangesAsync();

        var skills = await SubSkillMapAsync();
        var current = await _uow.WeeklyFocusSkills.ListAsync(
            f => f.ProgramId == dto.ProgramId && f.MonthKey == dto.MonthKey && f.WeekNumber == dto.WeekNumber);
        return current.Select(f => ToFocusDto(f, skills)).ToList();
    }

    public async Task<WeeklyDataEntryDto> RecordWeeklyScoreAsync(RecordWeeklyScoreDto dto)
    {
        var existing = (await _uow.WeeklyDataEntries.ListAsync(e =>
            e.ParticipantId == dto.ParticipantId && e.SubSkillId == dto.SubSkillId &&
            e.MonthKey == dto.MonthKey && e.WeekNumber == dto.WeekNumber)).FirstOrDefault();

        var weekDate = ParseDate(dto.WeekDate) ?? DateTime.UtcNow.Date;

        if (existing is null)
        {
            existing = new WeeklyDataEntry
            {
                ParticipantId = dto.ParticipantId,
                SubSkillId = dto.SubSkillId,
                SessionId = dto.SessionId,
                MonthKey = dto.MonthKey,
                WeekNumber = dto.WeekNumber,
                WeekDate = weekDate,
                Score = dto.Score,
                RecordedByStaffMemberId = dto.RecordedByStaffMemberId,
            };
            await _uow.WeeklyDataEntries.AddAsync(existing);
        }
        else
        {
            existing.Score = dto.Score;
            existing.WeekDate = weekDate;
            existing.SessionId = dto.SessionId;
            existing.RecordedByStaffMemberId = dto.RecordedByStaffMemberId;
            await _uow.WeeklyDataEntries.UpdateAsync(existing);
        }

        await _uow.SaveChangesAsync();
        return ToEntryDto(existing);
    }

    public async Task<StarMonthDto?> GetStarMonthAsync(Guid participantId, string monthKey)
    {
        if (await _uow.Participants.GetByIdAsync(participantId) is null) return null;

        var entries = await _uow.WeeklyDataEntries.ListAsync(e => e.ParticipantId == participantId && e.MonthKey == monthKey);
        var snaps = await _uow.MonthlyProgressSnapshots.ListAsync(s => s.ParticipantId == participantId && s.MonthKey == monthKey);
        var skills = await SubSkillMapAsync();

        return new StarMonthDto
        {
            ParticipantId = participantId,
            MonthKey = monthKey,
            Entries = entries
                .OrderBy(e => e.WeekNumber)
                .Select(ToEntryDto)
                .ToList(),
            Snapshots = snaps
                .OrderBy(s => skills.GetValueOrDefault(s.SubSkillId)?.SectionNumber ?? 0)
                .ThenBy(s => skills.GetValueOrDefault(s.SubSkillId)?.SortOrder ?? 0)
                .Select(s => ToSnapshotDto(s, skills))
                .ToList(),
        };
    }

    public async Task<IReadOnlyList<MonthlyProgressSnapshotDto>?> ComputeMonthEndAsync(Guid participantId, string monthKey)
    {
        if (await _uow.Participants.GetByIdAsync(participantId) is null) return null;

        var skills = (await _uow.SubSkills.GetAllAsync()).Where(s => s.IsActive).ToList();
        var entries = await _uow.WeeklyDataEntries.ListAsync(e => e.ParticipantId == participantId && e.MonthKey == monthKey);
        var thresholds = (await _uow.ScoreThresholds.GetAllAsync()).Select(t => (t.Level, t.MinAverage)).ToList();

        var snaps = (await _uow.MonthlyProgressSnapshots.ListAsync(s => s.ParticipantId == participantId && s.MonthKey == monthKey)).ToList();
        var bySkill = snaps.ToDictionary(s => s.SubSkillId);
        var entriesBySkill = entries.GroupBy(e => e.SubSkillId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var skill in skills)
        {
            var scores = entriesBySkill.TryGetValue(skill.Id, out var es)
                ? es.Select(e => e.Score)
                : Enumerable.Empty<Domain.Enums.DataScore>();
            var r = ProgressLevelCalculator.Derive(scores, thresholds);

            if (bySkill.TryGetValue(skill.Id, out var snap))
            {
                snap.SuggestedLevel = r.Level;
                snap.SummedScore = r.SummedScore;
                snap.ScoredWeekCount = r.ScoredWeekCount;
                if (!snap.IsConfirmed) snap.Level = r.Level; // never overwrite a confirmed level
                await _uow.MonthlyProgressSnapshots.UpdateAsync(snap);
            }
            else
            {
                snap = new MonthlyProgressSnapshot
                {
                    ParticipantId = participantId,
                    SubSkillId = skill.Id,
                    MonthKey = monthKey,
                    SuggestedLevel = r.Level,
                    Level = r.Level,
                    SummedScore = r.SummedScore,
                    ScoredWeekCount = r.ScoredWeekCount,
                    IsConfirmed = false,
                };
                await _uow.MonthlyProgressSnapshots.AddAsync(snap);
                snaps.Add(snap);
            }
        }

        await _uow.SaveChangesAsync();

        var skillMap = await SubSkillMapAsync();
        return snaps
            .OrderBy(s => skillMap.GetValueOrDefault(s.SubSkillId)?.SectionNumber ?? 0)
            .ThenBy(s => skillMap.GetValueOrDefault(s.SubSkillId)?.SortOrder ?? 0)
            .Select(s => ToSnapshotDto(s, skillMap))
            .ToList();
    }

    public async Task<MonthlyProgressSnapshotDto?> ConfirmMonthEndAsync(Guid participantId, string monthKey, ConfirmMonthEndDto dto)
    {
        if (await _uow.Participants.GetByIdAsync(participantId) is null) return null;

        var snap = (await _uow.MonthlyProgressSnapshots.ListAsync(s =>
            s.ParticipantId == participantId && s.MonthKey == monthKey && s.SubSkillId == dto.SubSkillId)).FirstOrDefault();

        if (snap is null)
        {
            snap = new MonthlyProgressSnapshot
            {
                ParticipantId = participantId,
                SubSkillId = dto.SubSkillId,
                MonthKey = monthKey,
                Level = dto.Level,
                SuggestedLevel = dto.Level,
                IsConfirmed = true,
                ConfirmedByStaffMemberId = dto.ConfirmedByStaffMemberId,
            };
            await _uow.MonthlyProgressSnapshots.AddAsync(snap);
        }
        else
        {
            snap.Level = dto.Level;
            snap.IsConfirmed = true;
            snap.ConfirmedByStaffMemberId = dto.ConfirmedByStaffMemberId;
            await _uow.MonthlyProgressSnapshots.UpdateAsync(snap);
        }

        await _uow.SaveChangesAsync();
        return ToSnapshotDto(snap, await SubSkillMapAsync());
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private async Task<Dictionary<Guid, SubSkill>> SubSkillMapAsync() =>
        (await _uow.SubSkills.GetAllAsync()).ToDictionary(s => s.Id);

    private static DateTime? ParseDate(string? s) =>
        DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var d) ? d : null;

    private static WeeklyDataEntryDto ToEntryDto(WeeklyDataEntry e) => new()
    {
        Id = e.Id,
        ParticipantId = e.ParticipantId,
        SubSkillId = e.SubSkillId,
        SessionId = e.SessionId,
        MonthKey = e.MonthKey,
        WeekNumber = e.WeekNumber,
        WeekDate = e.WeekDate.ToString("yyyy-MM-dd"),
        Score = e.Score,
        RecordedByStaffMemberId = e.RecordedByStaffMemberId,
    };

    private static MonthlyProgressSnapshotDto ToSnapshotDto(MonthlyProgressSnapshot s, Dictionary<Guid, SubSkill> skills)
    {
        var skill = skills.GetValueOrDefault(s.SubSkillId);
        return new MonthlyProgressSnapshotDto
        {
            Id = s.Id,
            ParticipantId = s.ParticipantId,
            SubSkillId = s.SubSkillId,
            SubSkillName = skill?.Name ?? "",
            SectionNumber = skill?.SectionNumber ?? 0,
            MonthKey = s.MonthKey,
            Level = s.Level,
            SuggestedLevel = s.SuggestedLevel,
            SummedScore = s.SummedScore,
            ScoredWeekCount = s.ScoredWeekCount,
            IsConfirmed = s.IsConfirmed,
            ConfirmedByStaffMemberId = s.ConfirmedByStaffMemberId,
        };
    }

    private static WeeklyFocusSkillDto ToFocusDto(WeeklyFocusSkill f, Dictionary<Guid, SubSkill> skills)
    {
        var skill = skills.GetValueOrDefault(f.SubSkillId);
        return new WeeklyFocusSkillDto
        {
            ProgramId = f.ProgramId,
            MonthKey = f.MonthKey,
            WeekNumber = f.WeekNumber,
            SubSkillId = f.SubSkillId,
            SubSkillName = skill?.Name ?? "",
            SectionNumber = skill?.SectionNumber ?? 0,
        };
    }
}
