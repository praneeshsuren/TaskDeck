namespace TaskDeck.Application.DTOs;

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
