using MediatR;

namespace TaskDeck.Application.Commands.Projects;

/// <summary>
/// Command to delete a project
/// </summary>
public record DeleteProjectCommand(
    Guid ProjectId,
    Guid RequestedById
) : IRequest<bool>;
