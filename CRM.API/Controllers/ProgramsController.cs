using System.Security.Claims;
using CRM.Application.DTOs.Programs;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramService _service;

    public ProgramsController(IProgramService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProgramSummaryDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    /// <summary>The caller's in-scope programs (all for an Admin; assigned programs for staff). Literal route wins over {slug}.</summary>
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<ProgramSummaryDto>>> GetMine() =>
        Ok(await _service.GetForUserAsync(CurrentUserId()));

    [HttpGet("{slug}")]
    public async Task<ActionResult<ProgramSummaryDto>> GetBySlug(string slug)
    {
        var result = await _service.GetBySlugAsync(slug);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{slug}/detail")]
    public async Task<ActionResult<ProgramDetailDto>> GetDetail(string slug)
    {
        var result = await _service.GetDetailAsync(slug);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "ManagementWrite")]
    public async Task<ActionResult<ProgramSummaryDto>> Create([FromBody] CreateProgramDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManagementWrite")]
    public async Task<ActionResult<ProgramSummaryDto>> Update(Guid id, [FromBody] UpdateProgramDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    private Guid CurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }
}
