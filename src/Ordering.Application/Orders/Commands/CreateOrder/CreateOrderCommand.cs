using MediatR;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyCollection<CreateOrderItemRequest> Items) : IRequest<CreateOrderResponse>;

public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity, decimal UnitPrice);

public sealed record CreateOrderResponse(Guid OrderId, decimal TotalAmount, string Status);
