using Microsoft.AspNetCore.SignalR;

namespace TaskDeck.Infrastructure.RealTime;

/// <summary>
/// Service for publishing messages via SignalR
/// </summary>
public class SignalRMessagePublisher<THub> where THub : Hub
{
    private readonly IHubContext<THub> _hubContext;

    public SignalRMessagePublisher(IHubContext<THub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Notify clients about a task update
    /// </summary>
    public async Task NotifyTaskUpdatedAsync(Guid projectId, object taskData)
    {
        await _hubContext.Clients.Group($"project-{projectId}")
            .SendAsync("TaskUpdated", taskData);
    }

    /// <summary>
    /// Notify clients about a new task
    /// </summary>
    public async Task NotifyTaskCreatedAsync(Guid projectId, object taskData)
    {
        await _hubContext.Clients.Group($"project-{projectId}")
            .SendAsync("TaskCreated", taskData);
    }

    /// <summary>
    /// Notify clients about a deleted task
    /// </summary>
    public async Task NotifyTaskDeletedAsync(Guid projectId, Guid taskId)
    {
        await _hubContext.Clients.Group($"project-{projectId}")
            .SendAsync("TaskDeleted", taskId);
    }
}
