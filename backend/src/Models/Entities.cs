namespace TaskDeck.Api.Models;

public class AppUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? FirebaseUid { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public virtual ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public virtual ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
    public virtual ICollection<Invitation> ReceivedInvitations { get; set; } = new List<Invitation>();
    public virtual ICollection<Invitation> SentInvitations { get; set; } = new List<Invitation>();
}

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    public Guid OwnerId { get; set; }

    public virtual AppUser Owner { get; set; } = null!;
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}

/// <summary>
/// Many-to-many relationship between users and projects
/// </summary>
public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ProjectRole Role { get; set; } = ProjectRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public virtual Project Project { get; set; } = null!;
    public virtual AppUser User { get; set; } = null!;
}

/// <summary>
/// Invitation to join a project
/// </summary>
public class Invitation
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid InvitedUserId { get; set; }
    public Guid InvitedByUserId { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }

    public virtual Project Project { get; set; } = null!;
    public virtual AppUser InvitedUser { get; set; } = null!;
    public virtual AppUser InvitedByUser { get; set; } = null!;
}

public enum ProjectRole
{
    Member = 0,
    Admin = 1
}

public enum InvitationStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2
}

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


    public Guid ProjectId { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? AssignedToId { get; set; }


    public virtual Project Project { get; set; } = null!;
    public virtual AppUser CreatedBy { get; set; } = null!;
    public virtual AppUser? AssignedTo { get; set; }
}

public enum TaskItemStatus
{
    Todo = 0,
    InProgress = 1,
    InReview = 2,
    Done = 3,
    Cancelled = 4
}

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}
