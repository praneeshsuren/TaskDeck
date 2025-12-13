using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskDeck.Application.Commands.Auth;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Api.Controllers;

/// <summary>
/// Controller for authentication endpoints
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Login with Firebase ID token
    /// </summary>
    [HttpPost("firebase")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginWithFirebase([FromBody] FirebaseLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.IdToken))
        {
            return BadRequest(new { code = "VALIDATION_ERROR", message = "ID token is required" });
        }

        var command = new LoginWithFirebaseCommand(request.IdToken);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            _logger.LogWarning("Failed Firebase login attempt");
            return Unauthorized(new { code = "UNAUTHORIZED", message = "Invalid or expired token" });
        }

        _logger.LogInformation("User {UserId} logged in successfully", result.User.Id);
        return Ok(result);
    }
}
