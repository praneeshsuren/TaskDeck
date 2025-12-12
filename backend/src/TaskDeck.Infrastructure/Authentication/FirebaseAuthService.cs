using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TaskDeck.Infrastructure.Authentication;

/// <summary>
/// Service for Firebase authentication operations
/// </summary>
public class FirebaseAuthService
{
    private readonly ILogger<FirebaseAuthService> _logger;
    private readonly FirebaseAuth _firebaseAuth;

    public FirebaseAuthService(IConfiguration configuration, ILogger<FirebaseAuthService> logger)
    {
        _logger = logger;

        var credentialsPath = configuration["Firebase:CredentialsPath"];

        if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }
        }

        _firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    /// <summary>
    /// Verify a Firebase ID token
    /// </summary>
    public async Task<FirebaseToken?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            return await _firebaseAuth.VerifyIdTokenAsync(idToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Firebase ID token");
            return null;
        }
    }

    /// <summary>
    /// Get user info from Firebase by UID
    /// </summary>
    public async Task<UserRecord?> GetUserAsync(string uid)
    {
        try
        {
            return await _firebaseAuth.GetUserAsync(uid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Firebase user: {Uid}", uid);
            return null;
        }
    }
}
