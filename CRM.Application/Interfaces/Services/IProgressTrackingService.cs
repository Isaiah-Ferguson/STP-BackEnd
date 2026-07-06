using CRM.Application.DTOs.Progress;

namespace CRM.Application.Interfaces.Services;

public interface IProgressTrackingService
{
    /// <summary>The focus skills set for a program across a month (all weeks).</summary>
    Task<IReadOnlyList<WeeklyFocusSkillDto>> GetFocusSkillsAsync(Guid programId, string monthKey);

    /// <summary>Replaces the focus skills for one program-week; returns the new set.</summary>
    Task<IReadOnlyList<WeeklyFocusSkillDto>> SetFocusSkillsAsync(SetFocusSkillsDto dto);

    /// <summary>Records (upserts) one weekly Data score for a Star on a sub-skill.</summary>
    Task<WeeklyDataEntryDto> RecordWeeklyScoreAsync(RecordWeeklyScoreDto dto);

    /// <summary>A Star's full month: weekly entries + month-end snapshots. Null if the participant doesn't exist.</summary>
    Task<StarMonthDto?> GetStarMonthAsync(Guid participantId, string monthKey);

    /// <summary>(Re)derives suggested month-end levels for every active sub-skill; preserves confirmed levels. Null if the participant doesn't exist.</summary>
    Task<IReadOnlyList<MonthlyProgressSnapshotDto>?> ComputeMonthEndAsync(Guid participantId, string monthKey);

    /// <summary>Confirms (or overrides) a Star's month-end level for one sub-skill. Null if the participant doesn't exist.</summary>
    Task<MonthlyProgressSnapshotDto?> ConfirmMonthEndAsync(Guid participantId, string monthKey, ConfirmMonthEndDto dto);

    /// <summary>Records (upserts) a Section-6 note for a Star in one week. Null if the participant doesn't exist.</summary>
    Task<WeeklyNoteSelectionDto?> UpsertNoteSelectionAsync(Guid participantId, string monthKey, UpsertNoteSelectionDto dto);

    /// <summary>Creates or updates a Star's monthly summary. Null if the participant doesn't exist.</summary>
    Task<MonthlySummaryDto?> UpsertMonthlySummaryAsync(Guid participantId, string monthKey, UpsertMonthlySummaryDto dto);
}
