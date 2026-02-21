using MediatR;
using Ordering.Application.Common.Models;

namespace Ordering.Application.Orders.Queries.ListOrders;

public sealed record ListOrdersQuery(
    int Page,
    int PageSize,
    OrderListFilters Filters,
    OrderSortBy SortBy,
    SortDirection SortDirection) : IRequest<PagedResult<OrderListItemResponse>>;

public sealed record OrderListItemResponse(
    Guid OrderId,
    Guid CustomerId,
    DateTime CreatedAtUtc,
    string Status,
    decimal TotalAmount,
    int ItemsCount);
