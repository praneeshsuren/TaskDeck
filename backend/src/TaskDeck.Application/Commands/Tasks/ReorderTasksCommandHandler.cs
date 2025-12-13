using MediatR;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Handler for ReorderTasksCommand
/// </summary>
public class ReorderTasksCommandHandler : IRequestHandler<ReorderTasksCommand, bool>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReorderTasksCommandHandler(
        ITaskRepository taskRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _taskRepository = taskRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> Handle(ReorderTasksCommand request, CancellationToken cancellationToken)
    {
        foreach (var orderItem in request.TaskOrders)
        {
            var task = await _taskRepository.GetByIdAsync(orderItem.TaskId, cancellationToken);
            if (task != null && task.ProjectId == request.ProjectId)
            {
                task.Order = orderItem.Order;
                task.UpdatedAt = _dateTimeProvider.UtcNow;
                await _taskRepository.UpdateAsync(task, cancellationToken);
            }
        }

        return true;
    }
}
