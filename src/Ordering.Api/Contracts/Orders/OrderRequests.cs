using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Contracts.Orders;

public sealed record CreateOrderRequest(
    [property: Required] Guid CustomerId,
    [property: MinLength(1)] IReadOnlyCollection<CreateOrderItemDto> Items);

public sealed record CreateOrderItemDto(
    [property: Required] Guid ProductId,
    [property: Range(1, int.MaxValue)] int Quantity,
    [property: Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal UnitPrice);

public sealed record ListOrdersRequest(
    int Page = 1,
    int PageSize = 10,
    string? Status = null,
    Guid? CustomerId = null,
    DateTime? CreatedFromUtc = null,
    DateTime? CreatedToUtc = null,
    string? SortBy = null,
    string? SortDirection = null);
