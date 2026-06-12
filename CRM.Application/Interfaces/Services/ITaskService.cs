using CRM.Application.DTOs.Tasks;

namespace CRM.Application.Interfaces.Services;

public interface ITaskService
{
    Task<IReadOnlyList<ProjectDto>> GetProjectsAsync();
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto);
    Task<ProjectTaskDto> AddTaskAsync(Guid projectId, CreateTaskDto dto);
    Task<ProjectTaskDto?> UpdateTaskAsync(Guid taskId, UpdateTaskDto dto);
}
