using MediatR;

namespace Ordering.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDetailsResponse?>;

public sealed record ProductDetailsResponse(
    Guid ProductId,
    string Sku,
    string Name,
    decimal Price,
    bool IsActive,
    DateTime CreatedAtUtc);
