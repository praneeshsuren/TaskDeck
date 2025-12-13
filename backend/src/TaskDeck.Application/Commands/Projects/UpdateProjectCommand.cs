using MediatR;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Application.Commands.Projects;

/// <summary>
/// Command to update an existing project
/// </summary>
public record UpdateProjectCommand(
    Guid ProjectId,
    string? Name,
    string? Description,
    string? Color,
    string? Icon,
    bool? IsArchived,
    Guid RequestedById
) : IRequest<ProjectDto?>;
