using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TaskDeck.Api.Hubs;
using TaskDeck.Application.Commands.Tasks;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Queries.Tasks;
using TaskDeck.Domain.Enums;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for task management endpoints
/// </summary>
[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<TasksHub> _hubContext;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        IMediator mediator,
        IHubContext<TasksHub> hubContext,
        ILogger<TasksController> logger)
    {
        _mediator = mediator;
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
    [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTasksByProject(Guid projectId)
    {
        var userId = GetCurrentUserId();
        var query = new GetTasksByProjectQuery(projectId, userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new task in a project
    /// </summary>
    [HttpPost("api/projects/{projectId:guid}/tasks")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto request)
    {
        var userId = GetCurrentUserId();
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            projectId,
            request.AssignedToId,
            userId
        );

        var result = await _mediator.Send(command);
        
        // Notify clients via SignalR
        await _hubContext.Clients.Group($"project-{projectId}")
            .SendAsync("TaskCreated", result);
        
        _logger.LogInformation("Task {TaskId} created in project {ProjectId} by user {UserId}", 
            result.Id, projectId, userId);
        
        return CreatedAtAction(nameof(GetTasksByProject), new { projectId }, result);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("api/tasks/{id:guid}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto request)
    {
        var userId = GetCurrentUserId();
        var command = new UpdateTaskCommand(
            id,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.Order,
            request.DueDate,
            request.AssignedToId,
            userId
        );

        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "Task not found" });
        }

        // Notify clients via SignalR
        await _hubContext.Clients.Group($"project-{result.ProjectId}")
            .SendAsync("TaskUpdated", result);

        _logger.LogInformation("Task {TaskId} updated by user {UserId}", id, userId);
        return Ok(result);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("api/tasks/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var userId = GetCurrentUserId();
        
        // Get task info first for SignalR notification
        var taskQuery = new GetTasksByProjectQuery(Guid.Empty, userId);
        
        var command = new DeleteTaskCommand(id, userId);
        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound(new { code = "NOT_FOUND", message = "Task not found" });
        }

        _logger.LogInformation("Task {TaskId} deleted by user {UserId}", id, userId);
        return NoContent();
    }

    /// <summary>
    /// Reorder tasks within a project
    /// </summary>
    [HttpPut("api/tasks/reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReorderTasks([FromBody] ReorderTasksRequest request)
    {
        var userId = GetCurrentUserId();
        var command = new ReorderTasksCommand(
            request.ProjectId,
            request.TaskOrders.Select(t => new TaskOrderItem(t.TaskId, t.Order)).ToList(),
            userId
        );

        var result = await _mediator.Send(command);
        
        _logger.LogInformation("Tasks reordered in project {ProjectId} by user {UserId}", 
            request.ProjectId, userId);
        
        return Ok(new { success = true });
    }
}

/// <summary>
/// Request DTO for reordering tasks
/// </summary>
public class ReorderTasksRequest
{
    public Guid ProjectId { get; set; }
    public List<TaskOrderRequest> TaskOrders { get; set; } = new();
}

/// <summary>
/// Individual task order item in reorder request
/// </summary>
public class TaskOrderRequest
{
    public Guid TaskId { get; set; }
    public int Order { get; set; }
}
