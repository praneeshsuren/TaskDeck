using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskDeck.Application.Interfaces;

namespace TaskDeck.Infrastructure.Authentication;

/// <summary>
/// Service for Firebase authentication operations
/// </summary>
public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly ILogger<FirebaseAuthService> _logger;
    private readonly FirebaseAuth? _firebaseAuth;

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
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }
        else
        {
            _logger.LogWarning("Firebase credentials not found. Firebase authentication will not work.");
            _firebaseAuth = null;
        }
    }

    /// <summary>
    /// Verify a Firebase ID token and return token info
    /// </summary>
    public async Task<FirebaseTokenInfo?> VerifyIdTokenAsync(string idToken)
    {
        if (_firebaseAuth == null)
        {
            _logger.LogError("Firebase Auth is not initialized");
            return null;
        }

        try
        {
            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
            
            // Get user record for additional info
            var userRecord = await _firebaseAuth.GetUserAsync(decodedToken.Uid);
            
            return new FirebaseTokenInfo
            {
                Uid = decodedToken.Uid,
                Email = userRecord.Email ?? "",
                DisplayName = userRecord.DisplayName,
                PhotoUrl = userRecord.PhotoUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Firebase ID token");
            return null;
        }
    }
}

