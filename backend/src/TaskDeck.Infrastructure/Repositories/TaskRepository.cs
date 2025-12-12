using Microsoft.EntityFrameworkCore;
using TaskDeck.Application.Interfaces;
using TaskDeck.Domain.Entities;
using TaskDeck.Infrastructure.Persistence;

namespace TaskDeck.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TaskItem operations
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly TaskDeckDbContext _context;

    public TaskRepository(TaskDeckDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.CreatedBy)
            .Where(t => t.AssignedToId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetMaxOrderAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var maxOrder = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .MaxAsync(t => (int?)t.Order, cancellationToken);

        return maxOrder ?? 0;
    }
}
