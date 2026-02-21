using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Contracts.Products;

public sealed record CreateProductRequest(
    [property: Required] string Sku,
    [property: Required] string Name,
    [property: Range(typeof(decimal), "0", "79228162514264337593543950335")] decimal Price);

public sealed record ListProductsRequest(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    bool? IsActive = null,
    string? SortBy = null,
    string? SortDirection = null);
