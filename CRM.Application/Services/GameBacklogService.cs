using CRM.Application.DTOs.Games;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;
using CRM.Domain.Enums;

namespace CRM.Application.Services;

public class GameBacklogService : IGameBacklogService
{
    private readonly IUnitOfWork _uow;

    public GameBacklogService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<GameIdeaDto>> GetIdeasAsync()
    {
        var ideas = await _uow.GameIdeas.GetAllAsync();
        var staff = await StaffNameMapAsync();
        return ideas.OrderBy(i => i.Name).Select(i => ToIdeaDto(i, staff)).ToList();
    }

    public async Task<GameIdeaDto> CreateIdeaAsync(CreateGameIdeaDto dto)
    {
        var idea = new GameIdea
        {
            Name = dto.Name,
            StatusNotes = dto.StatusNotes,
            SourceInspiration = dto.SourceInspiration,
            TargetCategory = dto.TargetCategory,
            TeacherSuggested = dto.TeacherSuggested,
            TeacherSuggestedId = dto.TeacherSuggestedId,
        };
        await _uow.GameIdeas.AddAsync(idea);
        await _uow.SaveChangesAsync();
        return ToIdeaDto(idea, await StaffNameMapAsync());
    }

    public async Task<GameIdeaDto?> PromoteIdeaAsync(Guid id)
    {
        var idea = await _uow.GameIdeas.GetByIdAsync(id);
        if (idea is null) return null;
        if (idea.PromotedGameId is not null) return ToIdeaDto(idea, await StaffNameMapAsync()); // already promoted

        // Draft library entry — Multi-Area / all tiers; refine it in the Games editor.
        var areas = await _uow.ObjectiveAreas.GetAllAsync();
        var areaId = (areas.FirstOrDefault(a => a.Slug == "multi-area") ?? areas.First()).Id;

        var game = new Game
        {
            Name = idea.Name,
            Source = GameSource.Suggested,
            Category = idea.TargetCategory ?? GameCategory.SuggestedAddition,
            Tiers = GameTier.All,
            PrimaryObjectiveAreaId = areaId,
            Description = idea.StatusNotes,
        };
        await _uow.Games.AddAsync(game);
        await _uow.SaveChangesAsync();

        idea.PromotedGameId = game.Id;
        await _uow.GameIdeas.UpdateAsync(idea);
        await _uow.SaveChangesAsync();

        return ToIdeaDto(idea, await StaffNameMapAsync());
    }

    public async Task<IReadOnlyList<AgeModificationDto>> GetAgeModsAsync()
    {
        var mods = await _uow.AgeModifications.GetAllAsync();
        var staff = await StaffNameMapAsync();
        return mods
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new AgeModificationDto
            {
                Id = m.Id,
                GameName = m.GameName,
                GroupAgeLevel = m.GroupAgeLevel,
                Modification = m.Modification,
                TeacherSuggested = m.TeacherSuggested,
                TeacherSuggestedId = m.TeacherSuggestedId,
                TeacherSuggestedName = m.TeacherSuggestedId is { } sid ? staff.GetValueOrDefault(sid) : null,
                GameId = m.GameId,
            })
            .ToList();
    }

    public async Task<AgeModificationDto> CreateAgeModAsync(CreateAgeModificationDto dto)
    {
        var mod = new AgeModification
        {
            GameName = dto.GameName,
            GroupAgeLevel = dto.GroupAgeLevel,
            Modification = dto.Modification,
            TeacherSuggested = dto.TeacherSuggested,
            TeacherSuggestedId = dto.TeacherSuggestedId,
            GameId = dto.GameId,
        };
        await _uow.AgeModifications.AddAsync(mod);
        await _uow.SaveChangesAsync();
        var staff = await StaffNameMapAsync();
        return new AgeModificationDto
        {
            Id = mod.Id,
            GameName = mod.GameName,
            GroupAgeLevel = mod.GroupAgeLevel,
            Modification = mod.Modification,
            TeacherSuggested = mod.TeacherSuggested,
            TeacherSuggestedId = mod.TeacherSuggestedId,
            TeacherSuggestedName = mod.TeacherSuggestedId is { } sid ? staff.GetValueOrDefault(sid) : null,
            GameId = mod.GameId,
        };
    }

    private async Task<Dictionary<Guid, string>> StaffNameMapAsync() =>
        (await _uow.Staff.GetAllAsync()).ToDictionary(s => s.Id, s => s.FullName);

    private static GameIdeaDto ToIdeaDto(GameIdea i, Dictionary<Guid, string> staff) => new()
    {
        Id = i.Id,
        Name = i.Name,
        StatusNotes = i.StatusNotes,
        SourceInspiration = i.SourceInspiration,
        TargetCategory = i.TargetCategory,
        TeacherSuggested = i.TeacherSuggested,
        TeacherSuggestedId = i.TeacherSuggestedId,
        TeacherSuggestedName = i.TeacherSuggestedId is { } sid ? staff.GetValueOrDefault(sid) : null,
        PromotedGameId = i.PromotedGameId,
    };
}
