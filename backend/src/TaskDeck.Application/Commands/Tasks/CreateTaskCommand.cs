using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Domain.Enums;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Command to create a new task
/// </summary>
public record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    Guid ProjectId,
    Guid? AssignedToId,
    Guid CreatedById
) : IRequest<TaskItemDto>;
