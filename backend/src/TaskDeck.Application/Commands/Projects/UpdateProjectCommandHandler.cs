using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Application.Commands.Projects;

/// <summary>
/// Handler for UpdateProjectCommand
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _projectRepository = projectRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProjectDto?> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return null;
        }

        // Verify ownership
        if (project.OwnerId != request.RequestedById)
        {
            return null;
        }

        // Update fields if provided
        if (request.Name != null)
            project.Name = request.Name;
        
        if (request.Description != null)
            project.Description = request.Description;
        
        if (request.Color != null)
            project.Color = request.Color;
        
        if (request.Icon != null)
            project.Icon = request.Icon;
        
        if (request.IsArchived.HasValue)
            project.IsArchived = request.IsArchived.Value;

        project.UpdatedAt = _dateTimeProvider.UtcNow;

        var updatedProject = await _projectRepository.UpdateAsync(project, cancellationToken);

        return new ProjectDto
        {
            Id = updatedProject.Id,
            Name = updatedProject.Name,
            Description = updatedProject.Description,
            Color = updatedProject.Color,
            Icon = updatedProject.Icon,
            IsArchived = updatedProject.IsArchived,
            CreatedAt = updatedProject.CreatedAt,
            UpdatedAt = updatedProject.UpdatedAt,
            Owner = new UserDto
            {
                Id = updatedProject.Owner.Id,
                Email = updatedProject.Owner.Email,
                DisplayName = updatedProject.Owner.DisplayName,
                AvatarUrl = updatedProject.Owner.AvatarUrl
            },
            TaskCount = updatedProject.Tasks?.Count ?? 0
        };
    }
}
