using Microsoft.AspNetCore.Mvc;
using TaskDeck.Api.Models;
using TaskDeck.Api.Services;

namespace TaskDeck.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("firebase")]
    public async Task<IActionResult> LoginWithFirebase([FromBody] FirebaseLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.IdToken))
        {
            return BadRequest(new { message = "ID token is required" });
        }

        try 
        {
            _logger.LogInformation("Processing Firebase login...");
            var result = await _authService.LoginWithFirebaseAsync(request.IdToken);

            if (result == null)
            {
                _logger.LogWarning("Firebase login failed - invalid token");
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            _logger.LogInformation("User {UserId} logged in successfully", result.User.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Firebase login");
            return StatusCode(500, new { message = "An unexpected error occurred" });
        }
    }
}
