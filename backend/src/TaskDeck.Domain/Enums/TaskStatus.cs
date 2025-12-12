namespace TaskDeck.Domain.Enums;

/// <summary>
/// Represents the status of a task
/// </summary>
public enum TaskItemStatus
{
    Todo = 0,
    InProgress = 1,
    InReview = 2,
    Done = 3,
    Cancelled = 4
}

/// <summary>
/// Represents the priority level of a task
/// </summary>
public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}
