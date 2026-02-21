using Ordering.Domain.Common;

namespace Ordering.Domain.Orders;

public sealed record OrderCreatedDomainEvent(Guid OrderId, DateTime OccurredOnUtc) : IDomainEvent;
