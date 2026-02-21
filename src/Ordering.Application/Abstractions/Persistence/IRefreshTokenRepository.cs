using Ordering.Domain.Security;

namespace Ordering.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<RefreshToken?> GetLatestActiveByUserIdAsync(string userId, CancellationToken cancellationToken);
}
