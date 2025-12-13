using MediatR;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Application.Commands.Projects;

/// <summary>
/// Handler for DeleteProjectCommand
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            return false;
        }

        // Verify ownership
        if (project.OwnerId != request.RequestedById)
        {
            return false;
        }

        await _projectRepository.DeleteAsync(request.ProjectId, cancellationToken);
        return true;
    }
}
