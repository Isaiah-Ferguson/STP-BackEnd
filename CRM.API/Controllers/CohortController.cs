using CRM.Application.DTOs.Progress;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/cohort")]
public class CohortController : ControllerBase
{
    private readonly ICohortRollUpService _service;

    public CohortController(ICohortRollUpService service) => _service = service;

    /// <summary>The cohort roll-up for a month (per-skill level counts from confirmed snapshots), optionally by program.</summary>
    [HttpGet("roll-up")]
    public async Task<ActionResult<CohortRollUpDto>> GetRollUp([FromQuery] string month, [FromQuery] Guid? programId)
    {
        if (string.IsNullOrWhiteSpace(month)) return BadRequest("month is required (yyyy-MM).");
        return Ok(await _service.GetRollUpAsync(month, programId));
    }
}
