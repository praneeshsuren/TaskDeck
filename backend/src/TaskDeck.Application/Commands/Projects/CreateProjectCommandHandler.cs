using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;
using TaskDeck.Domain.Entities;

namespace TaskDeck.Application.Commands.Projects;

/// <summary>
/// Handler for CreateProjectCommand
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner == null)
        {
            throw new InvalidOperationException("Owner not found");
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            Icon = request.Icon,
            OwnerId = request.OwnerId,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        var createdProject = await _projectRepository.CreateAsync(project, cancellationToken);

        return new ProjectDto
        {
            Id = createdProject.Id,
            Name = createdProject.Name,
            Description = createdProject.Description,
            Color = createdProject.Color,
            Icon = createdProject.Icon,
            IsArchived = createdProject.IsArchived,
            CreatedAt = createdProject.CreatedAt,
            UpdatedAt = createdProject.UpdatedAt,
            Owner = new UserDto
            {
                Id = owner.Id,
                Email = owner.Email,
                DisplayName = owner.DisplayName,
                AvatarUrl = owner.AvatarUrl
            },
            TaskCount = 0
        };
    }
}
