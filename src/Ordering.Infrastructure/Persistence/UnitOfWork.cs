using Ordering.Application.Abstractions.Persistence;

namespace Ordering.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly OrderingDbContext _dbContext;

    public UnitOfWork(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
