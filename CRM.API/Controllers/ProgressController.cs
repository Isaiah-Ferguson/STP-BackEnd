using System.Security.Claims;
using CRM.Application.DTOs.Progress;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly IProgressTrackingService _service;

    public ProgressController(IProgressTrackingService service) => _service = service;

    [HttpGet("focus-skills")]
    public async Task<ActionResult<IReadOnlyList<WeeklyFocusSkillDto>>> GetFocusSkills(
        [FromQuery] Guid programId, [FromQuery] string month)
    {
        if (programId == Guid.Empty || string.IsNullOrWhiteSpace(month)) return BadRequest("programId and month are required.");
        return Ok(await _service.GetFocusSkillsAsync(programId, month));
    }

    [HttpPut("focus-skills")]
    [Authorize(Policy = "ManagementWrite")]
    public async Task<ActionResult<IReadOnlyList<WeeklyFocusSkillDto>>> SetFocusSkills([FromBody] SetFocusSkillsDto dto)
    {
        if (dto.ProgramId == Guid.Empty || string.IsNullOrWhiteSpace(dto.MonthKey) || dto.WeekNumber < 1)
            return BadRequest("programId, monthKey and a weekNumber ≥ 1 are required.");
        return Ok(await _service.SetFocusSkillsAsync(dto));
    }

    [HttpPost("weekly")]
    public async Task<ActionResult<WeeklyDataEntryDto>> RecordWeekly([FromBody] RecordWeeklyScoreDto dto)
    {
        if (dto.ParticipantId == Guid.Empty || dto.SubSkillId == Guid.Empty || string.IsNullOrWhiteSpace(dto.MonthKey) || dto.WeekNumber < 1)
            return BadRequest("participantId, subSkillId, monthKey and a weekNumber ≥ 1 are required.");
        return Ok(await _service.RecordWeeklyScoreAsync(CurrentUserId(), dto));
    }

    [HttpGet("star/{participantId:guid}")]
    public async Task<ActionResult<StarMonthDto>> GetStarMonth(Guid participantId, [FromQuery] string month)
    {
        if (string.IsNullOrWhiteSpace(month)) return BadRequest("month is required.");
        var result = await _service.GetStarMonthAsync(participantId, month);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("star/{participantId:guid}/compute")]
    public async Task<ActionResult<IReadOnlyList<MonthlyProgressSnapshotDto>>> ComputeMonthEnd(Guid participantId, [FromQuery] string month)
    {
        if (string.IsNullOrWhiteSpace(month)) return BadRequest("month is required.");
        var result = await _service.ComputeMonthEndAsync(participantId, month);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("star/{participantId:guid}/confirm")]
    public async Task<ActionResult<MonthlyProgressSnapshotDto>> ConfirmMonthEnd(
        Guid participantId, [FromQuery] string month, [FromBody] ConfirmMonthEndDto dto)
    {
        if (string.IsNullOrWhiteSpace(month) || dto.SubSkillId == Guid.Empty) return BadRequest("month and subSkillId are required.");
        var result = await _service.ConfirmMonthEndAsync(CurrentUserId(), participantId, month, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("star/{participantId:guid}/note")]
    public async Task<ActionResult<WeeklyNoteSelectionDto>> UpsertNote(
        Guid participantId, [FromQuery] string month, [FromBody] UpsertNoteSelectionDto dto)
    {
        if (string.IsNullOrWhiteSpace(month) || dto.WeekNumber < 1) return BadRequest("month and a weekNumber ≥ 1 are required.");
        var result = await _service.UpsertNoteSelectionAsync(participantId, month, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("star/{participantId:guid}/summary")]
    public async Task<ActionResult<MonthlySummaryDto>> UpsertSummary(
        Guid participantId, [FromQuery] string month, [FromBody] UpsertMonthlySummaryDto dto)
    {
        if (string.IsNullOrWhiteSpace(month)) return BadRequest("month is required.");
        var result = await _service.UpsertMonthlySummaryAsync(participantId, month, dto);
        return result is null ? NotFound() : Ok(result);
    }

    private Guid CurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }
}
