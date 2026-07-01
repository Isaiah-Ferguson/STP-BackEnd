using CRM.Application.DTOs.Taxonomy;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

/// <summary>
/// Aggregate reference-list endpoint that mirrors the spreadsheets' "Do Not Remove-
/// Lists" tabs — the single call a form uses to populate its dropdowns. Grows with
/// staff, sites, and star groups in later phases.
/// </summary>
[ApiController]
[Authorize]
[Route("api/lists")]
public class ListsController : ControllerBase
{
    private readonly ITaxonomyService _service;

    public ListsController(ITaxonomyService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ReferenceListsDto>> GetLists() =>
        Ok(await _service.GetListsAsync());
}
