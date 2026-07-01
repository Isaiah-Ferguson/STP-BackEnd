using CRM.Application.DTOs.Taxonomy;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TaxonomyController : ControllerBase
{
    private readonly ITaxonomyService _service;

    public TaxonomyController(ITaxonomyService service) => _service = service;

    [HttpGet("objective-areas")]
    public async Task<ActionResult<IReadOnlyList<ObjectiveAreaDto>>> GetObjectiveAreas() =>
        Ok(await _service.GetObjectiveAreasAsync());

    [HttpGet("sub-skills")]
    public async Task<ActionResult<IReadOnlyList<SubSkillDto>>> GetSubSkills() =>
        Ok(await _service.GetSubSkillsAsync());
}
