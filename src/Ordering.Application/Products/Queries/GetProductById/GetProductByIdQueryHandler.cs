using MediatR;
using Ordering.Application.Abstractions.Persistence;

namespace Ordering.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailsResponse?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDetailsResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        return new ProductDetailsResponse(
            product.Id,
            product.Sku,
            product.Name,
            product.Price,
            product.IsActive,
            product.CreatedAtUtc);
    }
}
