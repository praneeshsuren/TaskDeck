namespace TaskDeck.Application.Interfaces;

/// <summary>
/// Interface for Firebase authentication operations
/// </summary>
public interface IFirebaseAuthService
{
    Task<FirebaseTokenInfo?> VerifyIdTokenAsync(string idToken);
}

/// <summary>
/// Information extracted from a Firebase token
/// </summary>
public class FirebaseTokenInfo
{
    public string Uid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
}
