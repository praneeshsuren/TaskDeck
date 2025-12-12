using TaskDeck.Application.Interfaces;

namespace TaskDeck.Infrastructure.Services;

/// <summary>
/// Implementation of IDateTimeProvider
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
