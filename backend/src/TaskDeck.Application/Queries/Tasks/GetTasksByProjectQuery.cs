using MediatR;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Application.Queries.Tasks;

/// <summary>
/// Query to get all tasks in a project
/// </summary>
public record GetTasksByProjectQuery(
    Guid ProjectId,
    Guid RequestedById
) : IRequest<IEnumerable<TaskItemDto>>;
