using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Application.Queries.Tasks;

/// <summary>
/// Handler for GetTasksByProjectQuery
/// </summary>
public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, IEnumerable<TaskItemDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public GetTasksByProjectQueryHandler(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<TaskItemDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        // Verify project access
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null || project.OwnerId != request.RequestedById)
        {
            return Enumerable.Empty<TaskItemDto>();
        }

        var tasks = await _taskRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        return tasks.Select(t => new TaskItemDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            Order = t.Order,
            DueDate = t.DueDate,
            CompletedAt = t.CompletedAt,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            ProjectId = t.ProjectId,
            AssignedTo = t.AssignedTo != null ? new UserDto
            {
                Id = t.AssignedTo.Id,
                Email = t.AssignedTo.Email,
                DisplayName = t.AssignedTo.DisplayName,
                AvatarUrl = t.AssignedTo.AvatarUrl
            } : null,
            CreatedBy = new UserDto
            {
                Id = t.CreatedBy.Id,
                Email = t.CreatedBy.Email,
                DisplayName = t.CreatedBy.DisplayName,
                AvatarUrl = t.CreatedBy.AvatarUrl
            }
        });
    }
}
