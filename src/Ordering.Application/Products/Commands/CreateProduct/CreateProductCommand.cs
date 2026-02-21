using MediatR;

namespace Ordering.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Sku, string Name, decimal Price) : IRequest<CreateProductResponse>;

public sealed record CreateProductResponse(Guid ProductId, string Sku, string Name, decimal Price, bool IsActive);
