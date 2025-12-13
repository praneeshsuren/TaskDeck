using MediatR;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Command to delete a task
/// </summary>
public record DeleteTaskCommand(
    Guid TaskId,
    Guid RequestedById
) : IRequest<bool>;
