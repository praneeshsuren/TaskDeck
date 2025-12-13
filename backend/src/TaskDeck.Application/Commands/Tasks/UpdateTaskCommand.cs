using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Domain.Enums;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Command to update an existing task
/// </summary>
public record UpdateTaskCommand(
    Guid TaskId,
    string? Title,
    string? Description,
    TaskItemStatus? Status,
    TaskPriority? Priority,
    int? Order,
    DateTime? DueDate,
    Guid? AssignedToId,
    Guid RequestedById
) : IRequest<TaskItemDto?>;
