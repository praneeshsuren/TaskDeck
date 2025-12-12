using Microsoft.AspNetCore.SignalR;

namespace TaskDeck.Api.Hubs;

/// <summary>
/// SignalR hub for real-time task updates
/// </summary>
public class TasksHub : Hub
{
    private readonly ILogger<TasksHub> _logger;

    public TasksHub(ILogger<TasksHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a project group to receive updates for that project
    /// </summary>
    public async Task JoinProject(string projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");
        _logger.LogInformation("Client {ConnectionId} joined project {ProjectId}", Context.ConnectionId, projectId);
    }

    /// <summary>
    /// Leave a project group
    /// </summary>
    public async Task LeaveProject(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project-{projectId}");
        _logger.LogInformation("Client {ConnectionId} left project {ProjectId}", Context.ConnectionId, projectId);
    }

    /// <summary>
    /// Notify clients about task updates
    /// </summary>
    public async Task NotifyTaskUpdated(string projectId, object taskData)
    {
        await Clients.Group($"project-{projectId}").SendAsync("TaskUpdated", taskData);
    }

    /// <summary>
    /// Notify clients about new tasks
    /// </summary>
    public async Task NotifyTaskCreated(string projectId, object taskData)
    {
        await Clients.Group($"project-{projectId}").SendAsync("TaskCreated", taskData);
    }

    /// <summary>
    /// Notify clients about deleted tasks
    /// </summary>
    public async Task NotifyTaskDeleted(string projectId, string taskId)
    {
        await Clients.Group($"project-{projectId}").SendAsync("TaskDeleted", taskId);
    }
}
