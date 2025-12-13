using MediatR;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Handler for DeleteTaskCommand
/// </summary>
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
        {
            return false;
        }

        await _taskRepository.DeleteAsync(request.TaskId, cancellationToken);
        return true;
    }
}
