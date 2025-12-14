namespace TaskDeck.Api.Models;

// ==================== User DTOs ====================

/// <summary>
/// DTO for user responses
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

// ==================== Auth DTOs ====================

/// <summary>
/// Request DTO for Firebase login
/// </summary>
public class FirebaseLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for authentication
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

// ==================== Project DTOs ====================

/// <summary>
/// DTO for project responses
/// </summary>
public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserDto Owner { get; set; } = null!;
    public int TaskCount { get; set; }
}

/// <summary>
/// DTO for creating a new project
/// </summary>
public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#3b82f6";
    public string Icon { get; set; } = "folder";
}

/// <summary>
/// DTO for updating a project
/// </summary>
public class UpdateProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool? IsArchived { get; set; }
}

// ==================== Task DTOs ====================

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

/// <summary>
/// DTO for reordering tasks
/// </summary>
public class ReorderTasksDto
{
    public List<TaskOrderItem> Tasks { get; set; } = new();
}

public class TaskOrderItem
{
    public Guid Id { get; set; }
    public int Order { get; set; }
}
