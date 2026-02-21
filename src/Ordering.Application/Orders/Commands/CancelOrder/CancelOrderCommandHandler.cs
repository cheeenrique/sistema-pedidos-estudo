using MediatR;
using Ordering.Application.Abstractions.Persistence;

namespace Ordering.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderResponse?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelOrderResponse?> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        order.Cancel();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelOrderResponse(order.Id, order.Status.ToString());
    }
}
