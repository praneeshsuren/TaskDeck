using MediatR;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Application.Queries.Projects;

/// <summary>
/// Query to get a project by ID
/// </summary>
public record GetProjectByIdQuery(
    Guid ProjectId,
    Guid RequestedById
) : IRequest<ProjectDto?>;
