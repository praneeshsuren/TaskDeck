using MediatR;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Application.Queries.Projects;

/// <summary>
/// Query to get all projects for a user
/// </summary>
public record GetProjectsQuery(
    Guid UserId
) : IRequest<IEnumerable<ProjectDto>>;
