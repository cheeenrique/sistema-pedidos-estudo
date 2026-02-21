using Ordering.Domain.Common;

namespace Ordering.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    private Order(Guid customerId, DateTime createdAtUtc)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAtUtc = createdAtUtc;
        Status = OrderStatus.Draft;
    }

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(item => item.Total);

    public static Order Create(Guid customerId, DateTime createdAtUtc)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer id must be a valid GUID.", nameof(customerId));
        }

        var order = new Order(customerId, createdAtUtc);
        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, createdAtUtc));
        return order;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
        {
            throw new InvalidOperationException("Only draft orders can be modified.");
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product id must be a valid GUID.", nameof(productId));
        }

        _items.Add(new OrderItem(productId, quantity, unitPrice));
    }

    public void Submit()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("Cannot submit an empty order.");
        }

        Status = OrderStatus.Submitted;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Shipped or delivered orders cannot be cancelled.");
        }

        if (Status == OrderStatus.Cancelled)
        {
            return;
        }

        Status = OrderStatus.Cancelled;
    }
}
