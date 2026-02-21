using MediatR;

namespace Ordering.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailsResponse?>;

public sealed record OrderDetailsResponse(
    Guid OrderId,
    Guid CustomerId,
    DateTime CreatedAtUtc,
    string Status,
    decimal TotalAmount,
    IReadOnlyCollection<OrderDetailsItemResponse> Items);

public sealed record OrderDetailsItemResponse(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);
