using CRM.Application.DTOs.Participants;

namespace CRM.Application.Interfaces.Services;

public interface IArtsProfileService
{
    /// <summary>The participant's Student Frame, or an empty default (HasProfile=false) if none set. Null if the participant doesn't exist.</summary>
    Task<ParticipantArtsProfileDto?> GetAsync(Guid participantId);

    /// <summary>Creates or updates the Student Frame. Null if the participant doesn't exist.</summary>
    Task<ParticipantArtsProfileDto?> UpsertAsync(Guid participantId, UpsertArtsProfileDto dto);
}
