using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskDeck.Api.Services;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for user management
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(AuthService authService, ILogger<UsersController> logger)
    {
        _authService = authService;
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
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        var result = await _authService.GetUserByIdAsync(userId);

        if (result == null)
            return NotFound(new { message = "User not found" });

        return Ok(result);
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _authService.GetUserByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "User not found" });

        return Ok(result);
    }
}
