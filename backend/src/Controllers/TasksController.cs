using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TaskDeck.Api.Hubs;
using TaskDeck.Api.Models;
using TaskDeck.Api.Services;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for task management
/// </summary>
[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;
    private readonly IHubContext<TasksHub> _hubContext;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        TaskService taskService,
        IHubContext<TasksHub> hubContext,
        ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _hubContext = hubContext;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get all tasks in a project
    /// </summary>
    [HttpGet("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> GetTasksByProject(Guid projectId)
    {
        var userId = GetCurrentUserId();
        var result = await _taskService.GetTasksByProjectAsync(projectId, userId);
        return Ok(result);
    }

    /// <summary>
    /// Create a new task in a project
    /// </summary>
    [HttpPost("api/projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto request)
    {
        var userId = GetCurrentUserId();
        request.ProjectId = projectId;
        var result = await _taskService.CreateTaskAsync(request, userId);
        
        // Notify clients via SignalR
        await _hubContext.Clients.Group($"project-{projectId}")
            .SendAsync("TaskCreated", result);
        
        _logger.LogInformation("Task {TaskId} created in project {ProjectId}", result.Id, projectId);
        return CreatedAtAction(nameof(GetTasksByProject), new { projectId }, result);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("api/tasks/{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto request)
    {
        var userId = GetCurrentUserId();
        var result = await _taskService.UpdateTaskAsync(id, request, userId);

        if (result == null)
            return NotFound(new { message = "Task not found" });

        // Notify clients via SignalR
        await _hubContext.Clients.Group($"project-{result.ProjectId}")
            .SendAsync("TaskUpdated", result);

        _logger.LogInformation("Task {TaskId} updated", id);
        return Ok(result);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("api/tasks/{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _taskService.DeleteTaskAsync(id, userId);

        if (!result)
            return NotFound(new { message = "Task not found" });

        _logger.LogInformation("Task {TaskId} deleted", id);
        return NoContent();
    }

    /// <summary>
    /// Reorder tasks within a project
    /// </summary>
    [HttpPut("api/tasks/reorder")]
    public async Task<IActionResult> ReorderTasks([FromBody] ReorderTasksDto request)
    {
        var userId = GetCurrentUserId();
        await _taskService.ReorderTasksAsync(request.Tasks, userId);
        return Ok();
    }
}
