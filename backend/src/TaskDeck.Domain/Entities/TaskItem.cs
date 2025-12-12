using TaskDeck.Domain.Enums;

namespace TaskDeck.Domain.Entities;

/// <summary>
/// Represents a task item within a project
/// </summary>
public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public int Order { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public Guid ProjectId { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? AssignedToId { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual AppUser CreatedBy { get; set; } = null!;
    public virtual AppUser? AssignedTo { get; set; }
}
