using CRM.Application.DTOs.Progress;
using CRM.Domain.Enums;

namespace CRM.Application.Interfaces.Services;

public interface IGoalBankService
{
    /// <summary>Active goal-bank entries, optionally filtered by section, level, and kind.</summary>
    Task<IReadOnlyList<GoalBankEntryDto>> GetAsync(int? sectionNumber, ProgressLevel? level, GoalBankKind? kind);
}
