using MediatR;
using TaskDeck.Application.DTOs;
using TaskDeck.Application.Interfaces;
using TaskDeck.Domain.Entities;

namespace TaskDeck.Application.Commands.Auth;

/// <summary>
/// Handler for LoginWithFirebaseCommand
/// </summary>
public class LoginWithFirebaseCommandHandler : IRequestHandler<LoginWithFirebaseCommand, AuthResponse?>
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginWithFirebaseCommandHandler(
        IFirebaseAuthService firebaseAuthService,
        IJwtService jwtService,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _firebaseAuthService = firebaseAuthService;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthResponse?> Handle(LoginWithFirebaseCommand request, CancellationToken cancellationToken)
    {
        // Verify Firebase token
        var firebaseInfo = await _firebaseAuthService.VerifyIdTokenAsync(request.IdToken);
        if (firebaseInfo == null)
        {
            return null;
        }

        // Find or create user
        var user = await _userRepository.GetByFirebaseUidAsync(firebaseInfo.Uid, cancellationToken);
        
        if (user == null)
        {
            // Check if user exists by email
            user = await _userRepository.GetByEmailAsync(firebaseInfo.Email, cancellationToken);
            
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
                    CreatedAt = _dateTimeProvider.UtcNow,
                    LastLoginAt = _dateTimeProvider.UtcNow
                };
                
                user = await _userRepository.CreateAsync(user, cancellationToken);
            }
            else
            {
                // Link Firebase UID to existing user
                user.FirebaseUid = firebaseInfo.Uid;
                user.LastLoginAt = _dateTimeProvider.UtcNow;
                if (!string.IsNullOrEmpty(firebaseInfo.PhotoUrl) && string.IsNullOrEmpty(user.AvatarUrl))
                {
                    user.AvatarUrl = firebaseInfo.PhotoUrl;
                }
                user = await _userRepository.UpdateAsync(user, cancellationToken);
            }
        }
        else
        {
            // Update last login
            user.LastLoginAt = _dateTimeProvider.UtcNow;
            user = await _userRepository.UpdateAsync(user, cancellationToken);
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        var expiresAt = _jwtService.GetTokenExpiration();

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl
            }
        };
    }
}
