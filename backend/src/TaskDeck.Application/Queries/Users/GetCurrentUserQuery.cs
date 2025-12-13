using MediatR;
using TaskDeck.Application.DTOs;

namespace TaskDeck.Application.Queries.Users;

/// <summary>
/// Query to get the current user's profile
/// </summary>
public record GetCurrentUserQuery(
    Guid UserId
) : IRequest<UserDto?>;
