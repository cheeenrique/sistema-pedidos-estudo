namespace Ordering.Domain.Orders;

public enum OrderStatus
{
    Draft = 1,
    Submitted = 2,
    Paid = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6
}
