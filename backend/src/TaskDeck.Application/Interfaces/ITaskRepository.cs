using TaskDeck.Domain.Entities;

namespace TaskDeck.Application.Interfaces;

/// <summary>
/// Repository interface for TaskItem operations
/// </summary>
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderAsync(Guid projectId, CancellationToken cancellationToken = default);
}
