using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskDeck.Application.Commands.Projects;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Queries.Projects;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for project management endpoints
/// </summary>
[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IMediator mediator, ILogger<ProjectsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Get all projects for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects()
    {
        var userId = GetCurrentUserId();
        var query = new GetProjectsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a project by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var userId = GetCurrentUserId();
        var query = new GetProjectByIdQuery(id, userId);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "Project not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto request)
    {
        var userId = GetCurrentUserId();
        var command = new CreateProjectCommand(
            request.Name,
            request.Description,
            request.Color,
            request.Icon,
            userId
        );

        var result = await _mediator.Send(command);
        _logger.LogInformation("Project {ProjectId} created by user {UserId}", result.Id, userId);
        
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto request)
    {
        var userId = GetCurrentUserId();
        var command = new UpdateProjectCommand(
            id,
            request.Name,
            request.Description,
            request.Color,
            request.Icon,
            request.IsArchived,
            userId
        );

        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "Project not found" });
        }

        _logger.LogInformation("Project {ProjectId} updated by user {UserId}", id, userId);
        return Ok(result);
    }

    /// <summary>
    /// Delete a project
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var userId = GetCurrentUserId();
        var command = new DeleteProjectCommand(id, userId);
        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound(new { code = "NOT_FOUND", message = "Project not found" });
        }

        _logger.LogInformation("Project {ProjectId} deleted by user {UserId}", id, userId);
        return NoContent();
    }
}
