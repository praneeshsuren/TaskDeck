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

        var credentialsPath = ResolveCredentialsPath(configuration["Firebase:CredentialsPath"]);
        _logger.LogInformation("Initializing Firebase Auth. Credentials path: {Path}, Exists: {Exists}", 
            credentialsPath, 
            !string.IsNullOrEmpty(credentialsPath) ? File.Exists(credentialsPath) : false);

        if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
        {
            try 
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    _logger.LogInformation("Creating new FirebaseApp instance");
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(credentialsPath)
                    });
                }
                else
                {
                    _logger.LogInformation("FirebaseApp instance already exists");
                }
                _firebaseAuth = FirebaseAuth.DefaultInstance;
                _logger.LogInformation("FirebaseAuth initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing FirebaseApp");
                _firebaseAuth = null;
            }
        }
        else
        {
            _logger.LogWarning("Firebase credentials not found at {Path}. Firebase authentication will not work.", credentialsPath);
            _firebaseAuth = null;
        }
    }

    /// <summary>
    /// Resolve the credentials file path, checking multiple possible locations
    /// </summary>
    private string? ResolveCredentialsPath(string? configuredPath)
    {
        if (string.IsNullOrEmpty(configuredPath))
        {
            return null;
        }

        // If it's an absolute path and exists, use it directly
        if (Path.IsPathRooted(configuredPath) && File.Exists(configuredPath))
        {
            return configuredPath;
        }

        // Try the configured path relative to current directory
        var currentDirPath = Path.GetFullPath(configuredPath);
        if (File.Exists(currentDirPath))
        {
            return currentDirPath;
        }

        // Try relative to AppContext.BaseDirectory (where the DLL runs from)
        var baseDirPath = Path.Combine(AppContext.BaseDirectory, configuredPath);
        if (File.Exists(baseDirPath))
        {
            return baseDirPath;
        }

        // Try common locations for the file
        var fileName = Path.GetFileName(configuredPath);
        var possibleLocations = new[]
        {
            Path.Combine(AppContext.BaseDirectory, fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", fileName)
        };

        foreach (var location in possibleLocations)
        {
            var fullPath = Path.GetFullPath(location);
            if (File.Exists(fullPath))
            {
                _logger.LogInformation("Found credentials file at alternative location: {Path}", fullPath);
                return fullPath;
            }
        }

        // Return the original configured path for error logging purposes
        return configuredPath;
    }

    /// <summary>
    /// Verify a Firebase ID token and return token info
    /// </summary>
    public async Task<FirebaseTokenInfo?> VerifyIdTokenAsync(string idToken)
    {
        if (_firebaseAuth == null)
        {
            _logger.LogError("Cannot verify token: Firebase Auth is not initialized. Check credentials file.");
            return null;
        }

        try
        {
            _logger.LogInformation("Verifying ID token...");
            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
            _logger.LogInformation("Token verified for UID: {Uid}", decodedToken.Uid);
            
            // Get user record for additional info
            var userRecord = await _firebaseAuth.GetUserAsync(decodedToken.Uid);
            _logger.LogInformation("Retrieved user record for email: {Email}", userRecord.Email);
            
            return new FirebaseTokenInfo
            {
                Uid = decodedToken.Uid,
                Email = userRecord.Email ?? "",
                DisplayName = userRecord.DisplayName,
                PhotoUrl = userRecord.PhotoUrl
            };
        }
        catch (FirebaseAuthException authEx)
        {
             _logger.LogError(authEx, "Firebase Auth Error: {Code} - {Message}", authEx.ErrorCode, authEx.Message);
             return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Firebase ID token. Invalid token or configuration mismatch.");
            return null;
        }
    }
}

