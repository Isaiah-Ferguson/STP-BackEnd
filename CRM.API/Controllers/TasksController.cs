using CRM.Application.DTOs.Tasks;
using CRM.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service) => _service = service;

    [HttpGet("projects")]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetProjects() =>
        Ok(await _service.GetProjectsAsync());

    // Defining org initiatives (productions, compliance drives) is management work;
    // adding/updating tasks within a project stays open to teachers (#6).
    [HttpPost("projects")]
    [Authorize(Policy = "ManagementWrite")]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto dto)
    {
        var result = await _service.CreateProjectAsync(dto);
        return CreatedAtAction(nameof(GetProjects), result);
    }

    [HttpPost("projects/{projectId:guid}/tasks")]
    public async Task<ActionResult<ProjectTaskDto>> AddTask(Guid projectId, [FromBody] CreateTaskDto dto)
    {
        var result = await _service.AddTaskAsync(projectId, dto);
        return CreatedAtAction(nameof(GetProjects), new { }, result);
    }

    [HttpPut("tasks/{taskId:guid}")]
    public async Task<ActionResult<ProjectTaskDto>> UpdateTask(Guid taskId, [FromBody] UpdateTaskDto dto)
    {
        var result = await _service.UpdateTaskAsync(taskId, dto);
        return result is null ? NotFound() : Ok(result);
    }
}
