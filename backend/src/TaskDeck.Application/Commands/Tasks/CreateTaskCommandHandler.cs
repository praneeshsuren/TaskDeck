using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;
using TaskDeck.Domain.Entities;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Handler for CreateTaskCommand
/// </summary>
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TaskItemDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var maxOrder = await _taskRepository.GetMaxOrderAsync(request.ProjectId, cancellationToken);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            AssignedToId = request.AssignedToId,
            CreatedById = request.CreatedById,
            Order = maxOrder + 1,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        var createdTask = await _taskRepository.CreateAsync(task, cancellationToken);
        var createdBy = await _userRepository.GetByIdAsync(request.CreatedById, cancellationToken);

        return new TaskItemDto
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            Description = createdTask.Description,
            Status = createdTask.Status,
            Priority = createdTask.Priority,
            Order = createdTask.Order,
            DueDate = createdTask.DueDate,
            CreatedAt = createdTask.CreatedAt,
            ProjectId = createdTask.ProjectId,
            CreatedBy = createdBy != null ? new UserDto
            {
                Id = createdBy.Id,
                Email = createdBy.Email,
                DisplayName = createdBy.DisplayName,
                AvatarUrl = createdBy.AvatarUrl
            } : null!
        };
    }
}
