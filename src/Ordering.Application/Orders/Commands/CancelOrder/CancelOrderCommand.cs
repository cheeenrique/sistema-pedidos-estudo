using MediatR;

namespace Ordering.Application.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : IRequest<CancelOrderResponse?>;

public sealed record CancelOrderResponse(Guid OrderId, string Status);
