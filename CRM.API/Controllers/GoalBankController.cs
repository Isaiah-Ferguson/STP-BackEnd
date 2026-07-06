using CRM.Application.DTOs.Progress;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize]
[Route("api/goal-bank")]
public class GoalBankController : ControllerBase
{
    private readonly IGoalBankService _service;

    public GoalBankController(IGoalBankService service) => _service = service;

    /// <summary>The Section-6 example bank, optionally filtered by section (1–5), level, and kind.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GoalBankEntryDto>>> Get(
        [FromQuery] int? section, [FromQuery] ProgressLevel? level, [FromQuery] GoalBankKind? kind)
        => Ok(await _service.GetAsync(section, level, kind));
}
