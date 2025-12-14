using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace TaskDeck.Api.Services;

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

/// <summary>
/// Service for Firebase authentication operations
/// </summary>
public class FirebaseAuthService
{
    private readonly ILogger<FirebaseAuthService> _logger;
    private readonly FirebaseAuth? _firebaseAuth;

    public FirebaseAuthService(IConfiguration configuration, ILogger<FirebaseAuthService> logger)
    {
        _logger = logger;

        var credentialsPath = ResolveCredentialsPath(configuration["Firebase:CredentialsPath"]);
        _logger.LogInformation("Initializing Firebase Auth. Credentials path: {Path}, Exists: {Exists}", 
            credentialsPath, 
            !string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath));

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
            _logger.LogWarning("Firebase credentials not found at {Path}", credentialsPath);
            _firebaseAuth = null;
        }
    }

    private string? ResolveCredentialsPath(string? configuredPath)
    {
        if (string.IsNullOrEmpty(configuredPath)) return null;

        // If absolute path exists, use it
        if (Path.IsPathRooted(configuredPath) && File.Exists(configuredPath))
            return configuredPath;

        // Try relative to current directory
        var currentDirPath = Path.GetFullPath(configuredPath);
        if (File.Exists(currentDirPath)) return currentDirPath;

        // Try relative to app base directory
        var baseDirPath = Path.Combine(AppContext.BaseDirectory, configuredPath);
        if (File.Exists(baseDirPath)) return baseDirPath;

        // Try common locations
        var fileName = Path.GetFileName(configuredPath);
        var locations = new[]
        {
            Path.Combine(AppContext.BaseDirectory, fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", fileName)
        };

        foreach (var location in locations)
        {
            var fullPath = Path.GetFullPath(location);
            if (File.Exists(fullPath))
            {
                _logger.LogInformation("Found credentials at: {Path}", fullPath);
                return fullPath;
            }
        }

        return configuredPath;
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
            var userRecord = await _firebaseAuth.GetUserAsync(decodedToken.Uid);
            
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
            _logger.LogError(authEx, "Firebase Auth Error: {Code}", authEx.ErrorCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Firebase ID token");
            return null;
        }
    }
}
