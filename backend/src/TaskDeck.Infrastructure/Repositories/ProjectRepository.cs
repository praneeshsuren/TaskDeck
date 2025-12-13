using Microsoft.EntityFrameworkCore;
using TaskDeck.Application.Interfaces;
using TaskDeck.Domain.Entities;
using TaskDeck.Infrastructure.Persistence;

namespace TaskDeck.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Project operations
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly TaskDeckDbContext _context;

    public ProjectRepository(TaskDeckDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .Where(p => p.OwnerId == ownerId && !p.IsArchived)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        project.UpdatedAt = DateTime.UtcNow;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> IsOwnerAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AnyAsync(p => p.Id == projectId && p.OwnerId == userId, cancellationToken);
    }
}
