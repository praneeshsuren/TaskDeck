using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;
using TaskDeck.Domain.Enums;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Handler for UpdateTaskCommand
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskItemDto?>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TaskItemDto?> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
        {
            return null;
        }

        // Update fields if provided
        if (request.Title != null)
            task.Title = request.Title;
        
        if (request.Description != null)
            task.Description = request.Description;
        
        if (request.Priority.HasValue)
            task.Priority = request.Priority.Value;
        
        if (request.Order.HasValue)
            task.Order = request.Order.Value;
        
        if (request.DueDate.HasValue)
            task.DueDate = request.DueDate.Value;
        
        if (request.AssignedToId.HasValue)
            task.AssignedToId = request.AssignedToId.Value;

        // Handle status change
        if (request.Status.HasValue && task.Status != request.Status.Value)
        {
            task.Status = request.Status.Value;
            if (request.Status.Value == TaskItemStatus.Done)
            {
                task.CompletedAt = _dateTimeProvider.UtcNow;
            }
            else if (task.CompletedAt.HasValue)
            {
                task.CompletedAt = null;
            }
        }

        task.UpdatedAt = _dateTimeProvider.UtcNow;

        var updatedTask = await _taskRepository.UpdateAsync(task, cancellationToken);

        // Reload to get navigation properties
        updatedTask = await _taskRepository.GetByIdAsync(updatedTask.Id, cancellationToken);

        return new TaskItemDto
        {
            Id = updatedTask!.Id,
            Title = updatedTask.Title,
            Description = updatedTask.Description,
            Status = updatedTask.Status,
            Priority = updatedTask.Priority,
            Order = updatedTask.Order,
            DueDate = updatedTask.DueDate,
            CompletedAt = updatedTask.CompletedAt,
            CreatedAt = updatedTask.CreatedAt,
            UpdatedAt = updatedTask.UpdatedAt,
            ProjectId = updatedTask.ProjectId,
            AssignedTo = updatedTask.AssignedTo != null ? new UserDto
            {
                Id = updatedTask.AssignedTo.Id,
                Email = updatedTask.AssignedTo.Email,
                DisplayName = updatedTask.AssignedTo.DisplayName,
                AvatarUrl = updatedTask.AssignedTo.AvatarUrl
            } : null,
            CreatedBy = new UserDto
            {
                Id = updatedTask.CreatedBy.Id,
                Email = updatedTask.CreatedBy.Email,
                DisplayName = updatedTask.CreatedBy.DisplayName,
                AvatarUrl = updatedTask.CreatedBy.AvatarUrl
            }
        };
    }
}
