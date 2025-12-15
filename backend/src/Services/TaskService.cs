using Microsoft.EntityFrameworkCore;
using TaskDeck.Api.Data;
using TaskDeck.Api.Models;

namespace TaskDeck.Api.Services;

/// <summary>
/// Service for task operations
/// </summary>
public class TaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Check if user has access to a project (owner or member)
    /// </summary>
    private async Task<bool> UserHasProjectAccessAsync(Guid projectId, Guid userId)
    {
        var project = await _db.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return false;

        return project.OwnerId == userId || project.Members.Any(m => m.UserId == userId);
    }

    /// <summary>
    /// Get all tasks in a project
    /// </summary>
    public async Task<List<TaskItemDto>> GetTasksByProjectAsync(Guid projectId, Guid userId)
    {
        // Verify user has access to the project (owner or member)
        var hasAccess = await UserHasProjectAccessAsync(projectId, userId);
        if (!hasAccess) return new List<TaskItemDto>();

        return await _db.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Order)
            .Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                Order = t.Order,
                DueDate = t.DueDate,
                CompletedAt = t.CompletedAt,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ProjectId = t.ProjectId,
                AssignedTo = t.AssignedTo != null ? new UserDto
                {
                    Id = t.AssignedTo.Id,
                    Email = t.AssignedTo.Email,
                    DisplayName = t.AssignedTo.DisplayName,
                    AvatarUrl = t.AssignedTo.AvatarUrl
                } : null,
                CreatedBy = new UserDto
                {
                    Id = t.CreatedBy.Id,
                    Email = t.CreatedBy.Email,
                    DisplayName = t.CreatedBy.DisplayName,
                    AvatarUrl = t.CreatedBy.AvatarUrl
                }
            })
            .ToListAsync();
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    public async Task<TaskItemDto?> CreateTaskAsync(CreateTaskDto dto, Guid userId)
    {
        // Verify user has access to the project
        var hasAccess = await UserHasProjectAccessAsync(dto.ProjectId, userId);
        if (!hasAccess) return null;

        var maxOrder = await _db.Tasks
            .Where(t => t.ProjectId == dto.ProjectId)
            .MaxAsync(t => (int?)t.Order) ?? 0;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
            AssignedToId = dto.AssignedToId,
            CreatedById = userId,
            Order = maxOrder + 1,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        var createdBy = await _db.Users.FindAsync(userId);

        return new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            Order = task.Order,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            ProjectId = task.ProjectId,
            CreatedBy = createdBy != null ? new UserDto
            {
                Id = createdBy.Id,
                Email = createdBy.Email,
                DisplayName = createdBy.DisplayName,
                AvatarUrl = createdBy.AvatarUrl
            } : null!
        };
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    public async Task<TaskItemDto?> UpdateTaskAsync(Guid id, UpdateTaskDto dto, Guid userId)
    {
        var task = await _db.Tasks
            .Include(t => t.Project)
                .ThenInclude(p => p.Members)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return null;

        // Check if user has access (owner or member)
        bool hasAccess = task.Project.OwnerId == userId || task.Project.Members.Any(m => m.UserId == userId);
        if (!hasAccess) return null;

        if (dto.Title != null) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Status.HasValue)
        {
            task.Status = dto.Status.Value;
            if (dto.Status.Value == TaskItemStatus.Done)
                task.CompletedAt = DateTime.UtcNow;
        }
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        if (dto.Order.HasValue) task.Order = dto.Order.Value;
        if (dto.DueDate.HasValue) task.DueDate = dto.DueDate.Value;
        if (dto.AssignedToId.HasValue) task.AssignedToId = dto.AssignedToId.Value;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new TaskItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            Order = task.Order,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            ProjectId = task.ProjectId,
            AssignedTo = task.AssignedTo != null ? new UserDto
            {
                Id = task.AssignedTo.Id,
                Email = task.AssignedTo.Email,
                DisplayName = task.AssignedTo.DisplayName,
                AvatarUrl = task.AssignedTo.AvatarUrl
            } : null,
            CreatedBy = new UserDto
            {
                Id = task.CreatedBy.Id,
                Email = task.CreatedBy.Email,
                DisplayName = task.CreatedBy.DisplayName,
                AvatarUrl = task.CreatedBy.AvatarUrl
            }
        };
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    public async Task<bool> DeleteTaskAsync(Guid id, Guid userId)
    {
        var task = await _db.Tasks
            .Include(t => t.Project)
                .ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return false;

        // Check if user has access (owner or member)
        bool hasAccess = task.Project.OwnerId == userId || task.Project.Members.Any(m => m.UserId == userId);
        if (!hasAccess) return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Reorder tasks within a project
    /// </summary>
    public async Task ReorderTasksAsync(List<TaskOrderItem> items, Guid userId)
    {
        foreach (var item in items)
        {
            var task = await _db.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Members)
                .FirstOrDefaultAsync(t => t.Id == item.Id);

            if (task != null)
            {
                // Check if user has access (owner or member)
                bool hasAccess = task.Project.OwnerId == userId || task.Project.Members.Any(m => m.UserId == userId);
                if (hasAccess)
                {
                    task.Order = item.Order;
                    task.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        await _db.SaveChangesAsync();
    }
}
