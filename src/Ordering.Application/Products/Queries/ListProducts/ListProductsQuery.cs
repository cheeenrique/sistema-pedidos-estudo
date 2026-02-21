using MediatR;
using Ordering.Application.Common.Models;

namespace Ordering.Application.Products.Queries.ListProducts;

public sealed record ListProductsQuery(
    int Page,
    int PageSize,
    string? Search,
    bool? IsActive,
    ProductSortBy SortBy,
    ProductSortDirection SortDirection) : IRequest<PagedResult<ProductListItemResponse>>;

public sealed record ProductListItemResponse(
    Guid ProductId,
    string Sku,
    string Name,
    decimal Price,
    bool IsActive,
    DateTime CreatedAtUtc);
