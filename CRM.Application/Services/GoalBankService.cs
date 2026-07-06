using CRM.Application.DTOs.Progress;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class GoalBankService : IGoalBankService
{
    private readonly IUnitOfWork _uow;

    public GoalBankService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<GoalBankEntryDto>> GetAsync(int? sectionNumber, ProgressLevel? level, GoalBankKind? kind)
    {
        var entries = await _uow.GoalBankEntries.GetAllAsync();
        return entries
            .Where(e => e.IsActive)
            .Where(e => sectionNumber is null || e.SectionNumber == sectionNumber)
            .Where(e => level is null || e.Level == level)
            .Where(e => kind is null || e.Kind == kind)
            .OrderBy(e => e.Kind)
            .ThenBy(e => e.SectionNumber)
            .ThenBy(e => e.Level)
            .Select(e => new GoalBankEntryDto
            {
                Id = e.Id,
                Kind = e.Kind,
                SectionNumber = e.SectionNumber,
                Level = e.Level,
                Text = e.Text,
                HasGrowingEdge = e.HasGrowingEdge,
            })
            .ToList();
    }
}
