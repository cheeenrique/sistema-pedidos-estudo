using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Products.Queries.ListProducts;
using Ordering.Domain.Products;

namespace Ordering.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly OrderingDbContext _dbContext;

    public ProductRepository(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return _dbContext.Products.FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<Product> Products, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        ProductSortBy sortBy,
        ProductSortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();
            query = query.Where(product =>
                product.Sku.ToLower().Contains(normalizedSearch) ||
                product.Name.ToLower().Contains(normalizedSearch));
        }

        if (isActive.HasValue)
        {
            query = query.Where(product => product.IsActive == isActive.Value);
        }

        var isDescending = sortDirection == ProductSortDirection.Desc;
        query = sortBy switch
        {
            ProductSortBy.Sku => isDescending
                ? query.OrderByDescending(product => product.Sku)
                : query.OrderBy(product => product.Sku),
            ProductSortBy.Name => isDescending
                ? query.OrderByDescending(product => product.Name)
                : query.OrderBy(product => product.Name),
            ProductSortBy.Price => isDescending
                ? query.OrderByDescending(product => product.Price)
                : query.OrderBy(product => product.Price),
            _ => isDescending
                ? query.OrderByDescending(product => product.CreatedAtUtc)
                : query.OrderBy(product => product.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }
}
