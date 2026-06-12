using CRM.Application.DTOs.Tasks;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Services;
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _uow;

    public TaskService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ProjectDto>> GetProjectsAsync()
    {
        var projects = await _uow.Projects.GetAllAsync();
        var tasks = await _uow.Tasks.GetAllAsync();
        var staff = await _uow.Staff.GetAllAsync();
        var staffMap = staff.ToDictionary(s => s.Id);
        var tasksByProject = tasks.GroupBy(t => t.ProjectId).ToDictionary(g => g.Key, g => g.ToList());

        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Type = p.Type,
            Status = p.Status,
            Scope = p.Scope,
            DueDate = p.DueDate?.ToString("yyyy-MM-dd"),
            Tasks = tasksByProject.GetValueOrDefault(p.Id, new())
                .Select(t => ToTaskDto(t, staffMap)).ToList(),
        }).ToList();
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Title = dto.Title,
            Type = dto.Type,
            Status = dto.Status,
            Scope = dto.Scope,
            DueDate = dto.DueDate,
        };

        await _uow.Projects.AddAsync(project);
        await _uow.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Type = project.Type,
            Status = project.Status,
            Scope = project.Scope,
            DueDate = project.DueDate?.ToString("yyyy-MM-dd"),
            Tasks = new(),
        };
    }

    public async Task<ProjectTaskDto> AddTaskAsync(Guid projectId, CreateTaskDto dto)
    {
        var task = new ProjectTask
        {
            ProjectId = projectId,
            Name = dto.Name,
            Context = dto.Context,
            AssignedToId = dto.AssignedToId,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
        };

        await _uow.Tasks.AddAsync(task);
        await _uow.SaveChangesAsync();

        return ToTaskDto(task, await BuildStaffMapAsync(task.AssignedToId));
    }

    public async Task<ProjectTaskDto?> UpdateTaskAsync(Guid taskId, UpdateTaskDto dto)
    {
        var task = await _uow.Tasks.GetByIdAsync(taskId);
        if (task is null) return null;

        if (dto.Name is not null) task.Name = dto.Name;
        if (dto.Context is not null) task.Context = dto.Context;
        if (dto.AssignedToId.HasValue) task.AssignedToId = dto.AssignedToId;
        if (dto.TaskStatus.HasValue) task.Status = dto.TaskStatus.Value;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        if (dto.DueDate.HasValue) task.DueDate = dto.DueDate;

        await _uow.Tasks.UpdateAsync(task);
        await _uow.SaveChangesAsync();

        return ToTaskDto(task, await BuildStaffMapAsync(task.AssignedToId));
    }

    private async Task<Dictionary<Guid, StaffMember>> BuildStaffMapAsync(Guid? assignedToId)
    {
        if (assignedToId is null) return new();
        var member = await _uow.Staff.GetByIdAsync(assignedToId.Value);
        return member is not null ? new() { [member.Id] = member } : new();
    }

    private static ProjectTaskDto ToTaskDto(ProjectTask t, Dictionary<Guid, StaffMember> staffMap)
    {
        var assignee = t.AssignedToId.HasValue ? staffMap.GetValueOrDefault(t.AssignedToId.Value) : null;
        return new ProjectTaskDto
        {
            Id = t.Id,
            ProjectId = t.ProjectId,
            Name = t.Name,
            Context = t.Context,
            AssignedToId = t.AssignedToId,
            AssignedToName = assignee?.FullName,
            AssignedToInitials = assignee?.Initials,
            TaskStatus = t.Status,
            Priority = t.Priority,
            DueDate = t.DueDate?.ToString("yyyy-MM-dd"),
            IsOverdue = t.IsOverdue,
        };
    }
}
