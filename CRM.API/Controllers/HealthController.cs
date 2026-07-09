using CRM.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

/// <summary>
/// Health-check endpoint. Verifies the database is reachable (#30) rather than
/// returning 200 unconditionally — a dead DB now reports 503 so monitors actually fire.
/// GET /api/health → 200 OK | 503 Service Unavailable
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        bool dbOk;
        try
        {
            dbOk = await _db.Database.CanConnectAsync(ct);
        }
        catch
        {
            dbOk = false;
        }

        var payload = new
        {
            Status = dbOk ? "Healthy" : "Unhealthy",
            Database = dbOk ? "Connected" : "Unreachable",
            Timestamp = DateTime.UtcNow,
            Service = "ShinyStarCRM API",
        };

        return dbOk ? Ok(payload) : StatusCode(StatusCodes.Status503ServiceUnavailable, payload);
    }
}
