using MediatR;

namespace TaskDeck.Application.Commands.Tasks;

/// <summary>
/// Command to reorder tasks within a project
/// </summary>
public record ReorderTasksCommand(
    Guid ProjectId,
    IList<TaskOrderItem> TaskOrders,
    Guid RequestedById
) : IRequest<bool>;

/// <summary>
/// Represents a task's new order position
/// </summary>
public record TaskOrderItem(Guid TaskId, int Order);
