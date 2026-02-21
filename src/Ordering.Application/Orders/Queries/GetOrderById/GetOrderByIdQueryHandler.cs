using MediatR;
using Ordering.Application.Abstractions.Persistence;

namespace Ordering.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailsResponse?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailsResponse?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return null;
        }

        var items = order.Items
            .Select(item => new OrderDetailsItemResponse(
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
                item.Total))
            .ToList();

        return new OrderDetailsResponse(
            order.Id,
            order.CustomerId,
            order.CreatedAtUtc,
            order.Status.ToString(),
            order.TotalAmount,
            items);
    }
}
