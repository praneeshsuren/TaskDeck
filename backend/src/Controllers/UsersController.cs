using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskDeck.Api.Services;

namespace TaskDeck.Api.Controllers;

/// Controller for user management
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

    /// Get the current user's profile
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        var result = await _authService.GetUserByIdAsync(userId);

        if (result == null)
            return NotFound(new { message = "User not found" });

        return Ok(result);
    }

    /// Get a user by ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _authService.GetUserByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "User not found" });

        return Ok(result);
    }
}
