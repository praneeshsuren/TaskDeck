using TaskDeck.Domain.Enums;

namespace TaskDeck.Application.DTOs;

/// <summary>
/// DTO for task item responses
/// </summary>
public class TaskItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int Order { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid ProjectId { get; set; }
    public UserDto? AssignedTo { get; set; }
    public UserDto CreatedBy { get; set; } = null!;
}

/// <summary>
/// DTO for creating a new task
/// </summary>
public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedToId { get; set; }
}

/// <summary>
/// DTO for updating a task
/// </summary>
public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public int? Order { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedToId { get; set; }
}
