using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Domain.Security;

namespace Ordering.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly OrderingDbContext _dbContext;

    public RefreshTokenRepository(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    public Task<RefreshToken?> GetLatestActiveByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAtUtc == null && token.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(token => token.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
