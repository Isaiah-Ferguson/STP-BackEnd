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
    [HttpGet("today")]
    public async Task<ActionResult<IReadOnlyList<AttendanceRosterEntryDto>>> GetToday() =>
        Ok(await _service.GetTodayRosterAsync());

    [HttpGet("session/{sessionId:guid}")]
    public async Task<ActionResult<AttendanceSessionDto>> GetSession(Guid sessionId)
    {
        var result = await _service.GetSessionAsync(sessionId);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{recordId:guid}")]
    public async Task<IActionResult> UpdateRecord(Guid recordId, [FromBody] UpdateAttendanceDto dto)
    {
        var updated = await _service.UpdateRecordAsync(recordId, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpPost("{recordId:guid}/notes")]
    public async Task<ActionResult<AttendanceNoteDto>> AddNote(Guid recordId, [FromBody] CreateAttendanceNoteDto dto)
    {
        var note = await _service.AddNoteAsync(recordId, dto);
        return note is null ? NotFound() : Ok(note);
    }
}
