using CRM.Application.DTOs.Programs;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramService _service;

    public ProgramsController(IProgramService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProgramSummaryDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

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
    public async Task<ActionResult<ProgramSummaryDto>> Create([FromBody] CreateProgramDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }
}
