namespace Ordering.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
