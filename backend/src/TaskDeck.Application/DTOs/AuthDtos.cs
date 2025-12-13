namespace TaskDeck.Application.DTOs;

/// <summary>
/// Request DTO for Firebase login
/// </summary>
public class FirebaseLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for authentication
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}
