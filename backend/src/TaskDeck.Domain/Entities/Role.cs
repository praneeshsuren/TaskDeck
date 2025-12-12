namespace TaskDeck.Domain.Entities;

/// <summary>
/// Represents a user role for authorization
/// </summary>
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Predefined roles
    public static class Names
    {
        public const string Admin = "Admin";
        public const string ProjectManager = "ProjectManager";
        public const string Developer = "Developer";
        public const string Viewer = "Viewer";
    }
}
