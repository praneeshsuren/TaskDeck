using TaskDeck.Domain.Entities;

namespace TaskDeck.Application.Interfaces;

/// <summary>
/// Repository interface for User operations
/// </summary>
public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<AppUser?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default);
    Task<IEnumerable<AppUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AppUser> CreateAsync(AppUser user, CancellationToken cancellationToken = default);
    Task<AppUser> UpdateAsync(AppUser user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
