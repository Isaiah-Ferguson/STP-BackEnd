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
        dto.RecordedByStaffMemberId ??= null; // stamped by client for now
        return Ok(await _service.RecordWeeklyScoreAsync(dto));
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
        dto.ConfirmedByStaffMemberId ??= null;
        var result = await _service.ConfirmMonthEndAsync(participantId, month, dto);
        return result is null ? NotFound() : Ok(result);
    }
}
