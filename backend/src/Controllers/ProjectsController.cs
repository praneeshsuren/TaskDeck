using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskDeck.Api.Models;
using TaskDeck.Api.Services;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for project management
/// </summary>
[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(ProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
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
    public async Task<IActionResult> GetProjects()
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.GetProjectsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Get a project by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.GetProjectByIdAsync(id, userId);

        if (result == null)
            return NotFound(new { message = "Project not found" });

        return Ok(result);
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto request)
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.CreateProjectAsync(request, userId);
        _logger.LogInformation("Project {ProjectId} created by user {UserId}", result.Id, userId);
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto request)
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.UpdateProjectAsync(id, request, userId);

        if (result == null)
            return NotFound(new { message = "Project not found" });

        _logger.LogInformation("Project {ProjectId} updated by user {UserId}", id, userId);
        return Ok(result);
    }

    /// <summary>
    /// Delete a project
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _projectService.DeleteProjectAsync(id, userId);

        if (!result)
            return NotFound(new { message = "Project not found" });

        _logger.LogInformation("Project {ProjectId} deleted by user {UserId}", id, userId);
        return NoContent();
    }
}
