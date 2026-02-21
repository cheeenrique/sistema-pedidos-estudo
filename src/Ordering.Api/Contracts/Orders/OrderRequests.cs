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

public sealed class ListOrdersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Status { get; init; }
    public Guid? CustomerId { get; init; }
    public DateTime? CreatedFromUtc { get; init; }
    public DateTime? CreatedToUtc { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}
