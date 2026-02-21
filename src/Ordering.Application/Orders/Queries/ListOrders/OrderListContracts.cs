namespace Ordering.Application.Orders.Queries.ListOrders;

public enum OrderSortBy
{
    CreatedAtUtc = 1,
    CustomerId = 2,
    Status = 3
}

public enum SortDirection
{
    Asc = 1,
    Desc = 2
}

public sealed record OrderListFilters(
    string? Status,
    Guid? CustomerId,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc);
