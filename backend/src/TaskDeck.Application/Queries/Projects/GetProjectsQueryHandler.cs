using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Application.Queries.Projects;

/// <summary>
/// Handler for GetProjectsQuery
/// </summary>
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);

        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Color = p.Color,
            Icon = p.Icon,
            IsArchived = p.IsArchived,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Owner = new UserDto
            {
                Id = p.Owner.Id,
                Email = p.Owner.Email,
                DisplayName = p.Owner.DisplayName,
                AvatarUrl = p.Owner.AvatarUrl
            },
            TaskCount = p.Tasks?.Count ?? 0
        });
    }
}
