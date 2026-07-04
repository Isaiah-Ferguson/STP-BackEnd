namespace CRM.Application.DTOs.Participants;

public class ParticipantArtsProfileDto
{
    public Guid ParticipantId { get; set; }
    public string? IppSummary { get; set; }
    public string? CurrentLevel { get; set; }
    public string? TsspArtsGoal { get; set; }
    /// <summary>False when no profile has been authored yet (the returned fields are empty defaults).</summary>
    public bool HasProfile { get; set; }
}

public class UpsertArtsProfileDto
{
    public string? IppSummary { get; set; }
    public string? CurrentLevel { get; set; }
    public string? TsspArtsGoal { get; set; }
}
