using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Application.Queries.Projects;

/// <summary>
/// Handler for GetProjectByIdQuery
/// </summary>
public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(request.ProjectId, cancellationToken);
        
        if (project == null || project.OwnerId != request.RequestedById)
        {
            return null;
        }

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Color = project.Color,
            Icon = project.Icon,
            IsArchived = project.IsArchived,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Owner = new UserDto
            {
                Id = project.Owner.Id,
                Email = project.Owner.Email,
                DisplayName = project.Owner.DisplayName,
                AvatarUrl = project.Owner.AvatarUrl
            },
            TaskCount = project.Tasks?.Count ?? 0
        };
    }
}
