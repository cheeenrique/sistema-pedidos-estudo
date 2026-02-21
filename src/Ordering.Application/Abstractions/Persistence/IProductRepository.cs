using Ordering.Application.Products.Queries.ListProducts;
using Ordering.Domain.Products;

namespace Ordering.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<Product> Products, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        ProductSortBy sortBy,
        ProductSortDirection sortDirection,
        CancellationToken cancellationToken);
}
