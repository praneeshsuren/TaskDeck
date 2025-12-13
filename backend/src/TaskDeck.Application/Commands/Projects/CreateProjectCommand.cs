using MediatR;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Application.Commands.Projects;

/// <summary>
/// Command to create a new project
/// </summary>
public record CreateProjectCommand(
    string Name,
    string? Description,
    string Color,
    string Icon,
    Guid OwnerId
) : IRequest<ProjectDto>;
