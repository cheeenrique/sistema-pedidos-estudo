using FluentAssertions;
using Ordering.Domain.Orders;

namespace Ordering.UnitTests.Domain;

public sealed class OrderTests
{
    [Fact]
    public void Submit_ShouldChangeStatusToSubmitted_WhenOrderHasItems()
    {
        var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow);
        order.AddItem(Guid.NewGuid(), 2, 30m);

        order.Submit();

        order.Status.Should().Be(OrderStatus.Submitted);
        order.TotalAmount.Should().Be(60m);
    }
}
