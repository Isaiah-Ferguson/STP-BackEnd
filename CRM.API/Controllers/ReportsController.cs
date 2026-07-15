using CRM.Application.DTOs.Reports;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _service;

    public ReportsController(IReportsService service) => _service = service;

    /// <summary>The org-wide reporting snapshot in one request.</summary>
    [HttpGet]
    public async Task<ActionResult<ReportsDto>> Get(CancellationToken ct) => Ok(await _service.GetAsync(ct));
}
