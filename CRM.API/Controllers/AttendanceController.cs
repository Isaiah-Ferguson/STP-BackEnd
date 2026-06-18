using System.Security.Claims;
using CRM.Application.DTOs.Attendance;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;

    public AttendanceController(IAttendanceService service) => _service = service;

    /// <summary>Today's attendance roster across all programs (created lazily if needed).</summary>
    /// <remarks>Deprecated — use <c>GET scheduled</c> + <c>GET session</c>. Retained until the frontend migrates.</remarks>
    [HttpGet("today")]
    [Obsolete("Superseded by GetScheduled + GetSessionByProgram.")]
    public async Task<ActionResult<IReadOnlyList<AttendanceRosterEntryDto>>> GetToday()
    {
#pragma warning disable CS0618
        return Ok(await _service.GetTodayRosterAsync());
#pragma warning restore CS0618
    }

    /// <summary>
    /// The session cards for a date (defaults to today), scoped to the caller's programs.
    /// Lists programs scheduled to meet that day plus any with a session already started.
    /// </summary>
    [HttpGet("scheduled")]
    public async Task<ActionResult<IReadOnlyList<ScheduledSessionDto>>> GetScheduled([FromQuery] DateTime? date)
    {
        var when = date?.Date ?? DateTime.UtcNow.Date;
        return Ok(await _service.GetScheduledForUserAsync(CurrentUserId(), when));
    }

    /// <summary>Gets or creates the session for a program on a date and returns its roster.</summary>
    [HttpGet("session")]
    public async Task<ActionResult<SessionRosterDto>> GetSessionByProgram(
        [FromQuery] Guid programId, [FromQuery] DateTime? date)
    {
        var when = date?.Date ?? DateTime.UtcNow.Date;
        try
        {
            var roster = await _service.GetOrCreateSessionAsync(CurrentUserId(), programId, when);
            return roster is null ? NotFound() : Ok(roster);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>Existing session roster by id (no creation).</summary>
    [HttpGet("session/{sessionId:guid}")]
    public async Task<ActionResult<AttendanceSessionDto>> GetSession(Guid sessionId)
    {
        var result = await _service.GetSessionAsync(sessionId);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Finalizes a session, locking its records.</summary>
    [HttpPost("session/{sessionId:guid}/submit")]
    public async Task<IActionResult> SubmitSession(Guid sessionId)
    {
        try
        {
            var ok = await _service.SubmitSessionAsync(CurrentUserId(), sessionId);
            return ok ? NoContent() : NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPut("{recordId:guid}")]
    public async Task<IActionResult> UpdateRecord(Guid recordId, [FromBody] UpdateAttendanceDto dto)
    {
        try
        {
            var updated = await _service.UpdateRecordAsync(recordId, dto);
            return updated ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{recordId:guid}/notes")]
    public async Task<ActionResult<AttendanceNoteDto>> AddNote(Guid recordId, [FromBody] CreateAttendanceNoteDto dto)
    {
        var note = await _service.AddNoteAsync(recordId, dto);
        return note is null ? NotFound() : Ok(note);
    }

    private Guid CurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }
}
