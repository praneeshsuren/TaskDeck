using System.Security.Claims;
using TaskDeck.Domain.Entities;

namespace TaskDeck.Application.Interfaces;

/// <summary>
/// Interface for JWT token operations
/// </summary>
public interface IJwtService
{
    string GenerateToken(AppUser user, IEnumerable<string>? roles = null);
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiration();
}
