using Microsoft.EntityFrameworkCore;
using TaskDeck.Api.Data;
using TaskDeck.Api.Models;

namespace TaskDeck.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly FirebaseAuthService _firebaseAuth;
    private readonly JwtService _jwt;

    public AuthService(AppDbContext db, FirebaseAuthService firebaseAuth, JwtService jwt)
    {
        _db = db;
        _firebaseAuth = firebaseAuth;
        _jwt = jwt;
    }

    /// Authenticate a user with a Firebase ID token
    public async Task<AuthResponse?> LoginWithFirebaseAsync(string idToken)
    {
        // Verify Firebase token
        var firebaseInfo = await _firebaseAuth.VerifyIdTokenAsync(idToken);
        if (firebaseInfo == null) return null;

        // Find or create user
        var user = await _db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseInfo.Uid);
        
        if (user == null)
        {
            // Check if user exists by email
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == firebaseInfo.Email);
            
            if (user == null)
            {
                // Create new user
                user = new AppUser
                {
                    Id = Guid.NewGuid(),
                    Email = firebaseInfo.Email,
                    DisplayName = firebaseInfo.DisplayName ?? firebaseInfo.Email.Split('@')[0],
                    AvatarUrl = firebaseInfo.PhotoUrl,
                    FirebaseUid = firebaseInfo.Uid,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
            }
            else
            {
                // Link Firebase UID to existing user
                user.FirebaseUid = firebaseInfo.Uid;
                user.LastLoginAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(firebaseInfo.PhotoUrl) && string.IsNullOrEmpty(user.AvatarUrl))
                {
                    user.AvatarUrl = firebaseInfo.PhotoUrl;
                }
            }
        }
        else
        {
            user.LastLoginAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        // Generate JWT token
        return new AuthResponse
        {
            Token = _jwt.GenerateToken(user),
            ExpiresAt = _jwt.GetTokenExpiration(),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl
            }
        };
    }

    /// Get user by ID
    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl
        };
    }
}
