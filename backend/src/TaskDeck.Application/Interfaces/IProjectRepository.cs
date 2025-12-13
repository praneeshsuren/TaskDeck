using TaskDeck.Domain.Entities;

namespace TaskDeck.Application.Interfaces;

/// <summary>
/// Repository interface for Project operations
/// </summary>
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsOwnerAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
}
