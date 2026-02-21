using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Api.Common.Responses;
using Ordering.Api.Contracts.Products;
using Ordering.Api.Validation;
using Ordering.Application.Common.Models;
using Ordering.Application.Products.Commands.CreateProduct;
using Ordering.Application.Products.Queries.GetProductById;
using Ordering.Application.Products.Queries.ListProducts;

namespace Ordering.Api.Controllers;

/// <summary>
/// Handles product catalog operations including pagination, creation, and details retrieval.
/// </summary>
[ApiController]
[Authorize]
[Route("api/products")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lists products with pagination, filters, and sorting.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "OrdersRead")]
    [ProducesResponseType(typeof(ApiSuccessResponse<PagedResult<ProductListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List([FromQuery] ListProductsRequest request, CancellationToken cancellationToken)
    {
        var errors = ProductRequestValidator.ValidateListRequest(request);
        if (errors.Count > 0)
        {
            return BadRequest(ApiResponseFactory.Error("ValidationError", "One or more validation errors occurred.", 400, HttpContext.TraceIdentifier, errors));
        }

        var query = new ListProductsQuery(
            request.Page,
            request.PageSize,
            request.Search,
            request.IsActive,
            ProductRequestValidator.ParseSortByOrDefault(request.SortBy),
            ProductRequestValidator.ParseSortDirectionOrDefault(request.SortDirection));

        var result = await _sender.Send(query, cancellationToken);
        return Ok(ApiResponseFactory.Success(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets product details by identifier.
    /// </summary>
    [HttpGet("{productId:guid}")]
    [Authorize(Policy = "OrdersRead")]
    [ProducesResponseType(typeof(ApiSuccessResponse<ProductDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductByIdQuery(productId), cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResponseFactory.Error("NotFound", "Product was not found.", 404, HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponseFactory.Success(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CatalogWrite")]
    [ProducesResponseType(typeof(ApiSuccessResponse<CreateProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var errors = ProductRequestValidator.ValidateCreateRequest(request);
        if (errors.Count > 0)
        {
            return BadRequest(ApiResponseFactory.Error("ValidationError", "One or more validation errors occurred.", 400, HttpContext.TraceIdentifier, errors));
        }

        var command = new CreateProductCommand(request.Sku, request.Name, request.Price);
        var result = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { productId = result.ProductId }, ApiResponseFactory.Success(result, HttpContext.TraceIdentifier, "Product created successfully."));
    }
}
