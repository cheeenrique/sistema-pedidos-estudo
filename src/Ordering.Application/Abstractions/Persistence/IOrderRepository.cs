using Ordering.Domain.Orders;
using Ordering.Application.Orders.Queries.ListOrders;

namespace Ordering.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<Order> Orders, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        OrderListFilters filters,
        OrderSortBy sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken);
}
