namespace TaskDeck.Application.DTOs;

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

/// <summary>
/// DTO for user profile responses
/// </summary>
public class UserProfileDto : UserDto
{
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
