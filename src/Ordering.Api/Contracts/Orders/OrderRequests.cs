using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Contracts.Orders;

public sealed class CreateOrderRequest
{
    [Required]
    public Guid CustomerId { get; init; }

    [MinLength(1)]
    public IReadOnlyCollection<CreateOrderItemDto> Items { get; init; } = Array.Empty<CreateOrderItemDto>();
}

public sealed class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal UnitPrice { get; init; }
}

public sealed record ListOrdersRequest(
    int Page = 1,
    int PageSize = 10,
    string? Status = null,
    Guid? CustomerId = null,
    DateTime? CreatedFromUtc = null,
    DateTime? CreatedToUtc = null,
    string? SortBy = null,
    string? SortDirection = null);
