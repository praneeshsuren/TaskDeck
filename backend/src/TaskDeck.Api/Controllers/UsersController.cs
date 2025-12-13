using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Queries.Users;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for user management endpoints
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
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
    /// Get the current user's profile
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        var query = new GetCurrentUserQuery(userId);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "User not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetCurrentUserQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "User not found" });
        }

        return Ok(result);
    }
}
