using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TaskDeck.Infrastructure.External;

/// <summary>
/// Client for Firebase Admin SDK operations
/// </summary>
public class FirebaseAdminClient
{
    private readonly ILogger<FirebaseAdminClient> _logger;
    private readonly FirebaseApp? _firebaseApp;

    public FirebaseAdminClient(IConfiguration configuration, ILogger<FirebaseAdminClient> logger)
    {
        _logger = logger;

        var credentialsPath = configuration["Firebase:CredentialsPath"];

        if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    _firebaseApp = FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(credentialsPath)
                    });
                }
                else
                {
                    _firebaseApp = FirebaseApp.DefaultInstance;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to initialize Firebase Admin SDK");
            }
        }
        else
        {
            _logger.LogWarning("Firebase credentials file not found at: {Path}", credentialsPath);
        }
    }

    /// <summary>
    /// Check if Firebase is initialized
    /// </summary>
    public bool IsInitialized => _firebaseApp != null;

    /// <summary>
    /// Set custom claims for a user
    /// </summary>
    public async Task SetCustomClaimsAsync(string uid, Dictionary<string, object> claims)
    {
        if (!IsInitialized) return;

        try
        {
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set custom claims for user: {Uid}", uid);
            throw;
        }
    }

    /// <summary>
    /// Revoke refresh tokens for a user
    /// </summary>
    public async Task RevokeRefreshTokensAsync(string uid)
    {
        if (!IsInitialized) return;

        try
        {
            await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(uid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke tokens for user: {Uid}", uid);
            throw;
        }
    }
}
