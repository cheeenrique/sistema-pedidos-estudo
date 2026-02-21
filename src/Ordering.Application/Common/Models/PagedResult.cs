namespace Ordering.Application.Common.Models;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    PaginationMetadata Pagination);
