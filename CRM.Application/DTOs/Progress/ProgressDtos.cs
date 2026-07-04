using CRM.Domain.Enums;

namespace CRM.Application.DTOs.Progress;

public class WeeklyDataEntryDto
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid SubSkillId { get; set; }
    public Guid? SessionId { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public string WeekDate { get; set; } = string.Empty;
    public DataScore Score { get; set; }
    public Guid? RecordedByStaffMemberId { get; set; }
}

public class RecordWeeklyScoreDto
{
    public Guid ParticipantId { get; set; }
    public Guid SubSkillId { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public string? WeekDate { get; set; }  // ISO date; defaults to today if omitted
    public DataScore Score { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? RecordedByStaffMemberId { get; set; }
}

public class MonthlyProgressSnapshotDto
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid SubSkillId { get; set; }
    public string SubSkillName { get; set; } = string.Empty;
    public int SectionNumber { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public ProgressLevel Level { get; set; }
    public ProgressLevel SuggestedLevel { get; set; }
    public int SummedScore { get; set; }
    public int ScoredWeekCount { get; set; }
    public bool IsConfirmed { get; set; }
    public Guid? ConfirmedByStaffMemberId { get; set; }
}

public class ConfirmMonthEndDto
{
    public Guid SubSkillId { get; set; }
    public ProgressLevel Level { get; set; }
    public Guid? ConfirmedByStaffMemberId { get; set; }
}

public class WeeklyFocusSkillDto
{
    public Guid ProgramId { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public Guid SubSkillId { get; set; }
    public string SubSkillName { get; set; } = string.Empty;
    public int SectionNumber { get; set; }
}

public class SetFocusSkillsDto
{
    public Guid ProgramId { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public List<Guid> SubSkillIds { get; set; } = new();
}

/// <summary>A Star's full month: every weekly entry plus the per-skill month-end snapshots.</summary>
public class StarMonthDto
{
    public Guid ParticipantId { get; set; }
    public string MonthKey { get; set; } = string.Empty;
    public List<WeeklyDataEntryDto> Entries { get; set; } = new();
    public List<MonthlyProgressSnapshotDto> Snapshots { get; set; } = new();
}
