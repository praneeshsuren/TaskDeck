using MediatR;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Application.Commands.Auth;

/// <summary>
/// Command to login with Firebase ID token
/// </summary>
public record LoginWithFirebaseCommand(
    string IdToken
) : IRequest<AuthResponse?>;
