using CRM.Application.DTOs.Participants;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class ParticipantService : IParticipantService
{
    private readonly IUnitOfWork _uow;

    public ParticipantService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ParticipantSummaryDto>> GetAllAsync()
    {
        var participants = await _uow.Participants.GetAllAsync();
        var programs = await _uow.Programs.GetAllAsync();
        var programMap = programs.ToDictionary(p => p.Id, p => p.Name);

        var slugMap = programs.ToDictionary(p => p.Id, p => p.Slug);
        return participants.Select(p => ToSummary(p, programMap, slugMap)).ToList();
    }

    public async Task<ParticipantDetailDto?> GetByIdAsync(Guid id)
    {
        var p = await _uow.Participants.GetByIdAsync(id);
        if (p is null) return null;

        var programs = await _uow.Programs.GetAllAsync();
        var prog = programs.FirstOrDefault(pr => pr.Id == p.ProgramId);

        return new ParticipantDetailDto
        {
            Id = p.Id,
            FullName = p.FullName,
            Initials = p.Initials,
            Status = p.Status,
            ProgramId = p.ProgramId,
            ProgramName = prog?.Name ?? string.Empty,
            ProgramSlug = prog?.Slug ?? string.Empty,
            AttendancePct = p.AttendancePct,
            StartDate = p.StartDate.ToString("yyyy-MM-dd"),
            HasDocAlerts = false,
            BirthYear = p.BirthYear,
            ServiceCoordinator = p.ServiceCoordinator,
            Documents = new(),
            RecentAttendance = new(),
        };
    }

    public async Task<ParticipantDetailDto> CreateAsync(CreateParticipantDto dto)
    {
        var participant = new Participant
        {
            FullName = dto.FullName,
            Initials = dto.Initials,
            ProgramId = dto.ProgramId,
            Status = dto.Status,
            BirthYear = dto.BirthYear,
            ServiceCoordinator = dto.ServiceCoordinator,
            StartDate = dto.StartDate ?? DateTime.UtcNow,
        };

        await _uow.Participants.AddAsync(participant);
        await _uow.SaveChangesAsync();

        return (await GetByIdAsync(participant.Id))!;
    }

    public async Task<ParticipantDetailDto?> UpdateAsync(Guid id, UpdateParticipantDto dto)
    {
        var participant = await _uow.Participants.GetByIdAsync(id);
        if (participant is null) return null;

        if (dto.FullName is not null) participant.FullName = dto.FullName;
        if (dto.Initials is not null) participant.Initials = dto.Initials;
        if (dto.ProgramId.HasValue) participant.ProgramId = dto.ProgramId.Value;
        if (dto.Status.HasValue) participant.Status = dto.Status.Value;
        if (dto.BirthYear.HasValue) participant.BirthYear = dto.BirthYear;
        if (dto.ServiceCoordinator is not null) participant.ServiceCoordinator = dto.ServiceCoordinator;

        await _uow.Participants.UpdateAsync(participant);
        await _uow.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var participant = await _uow.Participants.GetByIdAsync(id);
        if (participant is null) return false;

        await _uow.Participants.DeleteAsync(participant);
        await _uow.SaveChangesAsync();
        return true;
    }

    private static ParticipantSummaryDto ToSummary(
        Participant p,
        Dictionary<Guid, string> programMap,
        Dictionary<Guid, string>? slugMap = null) =>
        new()
        {
            Id = p.Id,
            FullName = p.FullName,
            Initials = p.Initials,
            Status = p.Status,
            ProgramId = p.ProgramId,
            ProgramName = programMap.GetValueOrDefault(p.ProgramId, string.Empty),
            ProgramSlug = slugMap?.GetValueOrDefault(p.ProgramId, string.Empty) ?? string.Empty,
            AttendancePct = p.AttendancePct,
            StartDate = p.StartDate.ToString("yyyy-MM-dd"),
            HasDocAlerts = false,
        };
}
