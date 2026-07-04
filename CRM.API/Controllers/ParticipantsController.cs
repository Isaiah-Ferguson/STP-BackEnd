using CRM.Application.DTOs.Participants;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    private readonly IParticipantService _service;
    private readonly IArtsProfileService _artsProfile;

    public ParticipantsController(IParticipantService service, IArtsProfileService artsProfile)
    {
        _service = service;
        _artsProfile = artsProfile;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ParticipantSummaryDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ParticipantDetailDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ParticipantDetailDto>> Create([FromBody] CreateParticipantDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ParticipantDetailDto>> Update(Guid id, [FromBody] UpdateParticipantDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>The participant's Student Frame (IPP summary, current level, TSSP arts goal).</summary>
    [HttpGet("{id:guid}/arts-profile")]
    public async Task<ActionResult<ParticipantArtsProfileDto>> GetArtsProfile(Guid id)
    {
        var profile = await _artsProfile.GetAsync(id);
        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut("{id:guid}/arts-profile")]
    public async Task<ActionResult<ParticipantArtsProfileDto>> UpsertArtsProfile(Guid id, [FromBody] UpsertArtsProfileDto dto)
    {
        var profile = await _artsProfile.UpsertAsync(id, dto);
        return profile is null ? NotFound() : Ok(profile);
    }
}
