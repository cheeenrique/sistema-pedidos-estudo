using MediatR;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Common.Models;

namespace Ordering.Application.Products.Queries.ListProducts;

public sealed class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, PagedResult<ProductListItemResponse>>
{
    private const int MaxPageSize = 100;
    private readonly IProductRepository _productRepository;

    public ListProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductListItemResponse>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, MaxPageSize);

        var (products, totalCount) = await _productRepository.ListPagedAsync(
            page,
            pageSize,
            request.Search,
            request.IsActive,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var items = products
            .Select(product => new ProductListItemResponse(
                product.Id,
                product.Sku,
                product.Name,
                product.Price,
                product.IsActive,
                product.CreatedAtUtc))
            .ToList();

        var totalPages = pageSize == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);
        var pagination = new PaginationMetadata(page, pageSize, totalCount, totalPages);

        return new PagedResult<ProductListItemResponse>(items, pagination);
    }
}
