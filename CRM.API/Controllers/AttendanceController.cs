using CRM.Application.DTOs.Attendance;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;

    public AttendanceController(IAttendanceService service) => _service = service;

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
}
