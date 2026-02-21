using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Orders.Queries.ListOrders;
using Ordering.Domain.Orders;

namespace Ordering.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _dbContext;

    public OrderRepository(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return _dbContext.Orders.FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<Order> Orders, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        OrderListFilters filters,
        OrderSortBy sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Orders.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Status) && Enum.TryParse<OrderStatus>(filters.Status, true, out var parsedStatus))
        {
            query = query.Where(order => order.Status == parsedStatus);
        }

        if (filters.CustomerId.HasValue && filters.CustomerId.Value != Guid.Empty)
        {
            query = query.Where(order => order.CustomerId == filters.CustomerId.Value);
        }

        if (filters.CreatedFromUtc.HasValue)
        {
            query = query.Where(order => order.CreatedAtUtc >= filters.CreatedFromUtc.Value);
        }

        if (filters.CreatedToUtc.HasValue)
        {
            query = query.Where(order => order.CreatedAtUtc <= filters.CreatedToUtc.Value);
        }

        var isDescending = sortDirection == SortDirection.Desc;
        query = sortBy switch
        {
            OrderSortBy.CustomerId => isDescending
                ? query.OrderByDescending(order => order.CustomerId)
                : query.OrderBy(order => order.CustomerId),
            OrderSortBy.Status => isDescending
                ? query.OrderByDescending(order => order.Status)
                : query.OrderBy(order => order.Status),
            _ => isDescending
                ? query.OrderByDescending(order => order.CreatedAtUtc)
                : query.OrderBy(order => order.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (orders, totalCount);
    }
}
