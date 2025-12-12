namespace TaskDeck.Application.Interfaces;

/// <summary>
/// Provides current date/time abstraction for testability
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
