using MediatR;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Common.Models;

namespace Ordering.Application.Orders.Queries.ListOrders;

public sealed class ListOrdersQueryHandler : IRequestHandler<ListOrdersQuery, PagedResult<OrderListItemResponse>>
{
    private const int MaxPageSize = 100;
    private readonly IOrderRepository _orderRepository;

    public ListOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderListItemResponse>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, MaxPageSize);

        var (orders, totalCount) = await _orderRepository.ListPagedAsync(
            page,
            pageSize,
            request.Filters,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var items = orders
            .Select(order => new OrderListItemResponse(
                order.Id,
                order.CustomerId,
                order.CreatedAtUtc,
                order.Status.ToString(),
                order.TotalAmount,
                order.Items.Count))
            .ToList();

        var totalPages = pageSize == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);
        var pagination = new PaginationMetadata(page, pageSize, totalCount, totalPages);
        return new PagedResult<OrderListItemResponse>(items, pagination);
    }
}
