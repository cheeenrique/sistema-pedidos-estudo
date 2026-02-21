using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Api.Contracts.Orders;
using Ordering.Api.Common.Responses;
using Ordering.Application.Common.Models;
using Ordering.Application.Orders.Commands.CancelOrder;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Application.Orders.Queries.GetOrderById;
using Ordering.Application.Orders.Queries.ListOrders;
using Ordering.Api.Validation;

namespace Ordering.Api.Controllers;

/// <summary>
/// Handles order operations such as listing, creating, reading details, and cancellation.
/// </summary>
[ApiController]
[Authorize]
[Route("api/orders")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
public sealed class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lists orders with pagination, filters, and sorting.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "OrdersRead")]
    [ProducesResponseType(typeof(ApiSuccessResponse<PagedResult<OrderListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List(
        [FromQuery] ListOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = OrderRequestValidator.ValidateListRequest(request);
        if (validationErrors.Count > 0)
        {
            var validationResponse = ApiResponseFactory.Error(
                "ValidationError",
                "One or more validation errors occurred.",
                StatusCodes.Status400BadRequest,
                HttpContext.TraceIdentifier,
                validationErrors);

            return BadRequest(validationResponse);
        }

        var filters = new OrderListFilters(
            request.Status,
            request.CustomerId,
            request.CreatedFromUtc,
            request.CreatedToUtc);

        var query = new ListOrdersQuery(
            request.Page,
            request.PageSize,
            filters,
            OrderRequestValidator.ParseSortByOrDefault(request.SortBy),
            OrderRequestValidator.ParseSortDirectionOrDefault(request.SortDirection));

        var result = await _sender.Send(query, cancellationToken);
        return Ok(ApiResponseFactory.Success(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets a specific order by identifier.
    /// </summary>
    [HttpGet("{orderId:guid}")]
    [Authorize(Policy = "OrdersRead")]
    [ProducesResponseType(typeof(ApiSuccessResponse<OrderDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid orderId, CancellationToken cancellationToken)
    {
        if (orderId == Guid.Empty)
        {
            var badRequestResponse = ApiResponseFactory.Error(
                "ValidationError",
                "One or more validation errors occurred.",
                StatusCodes.Status400BadRequest,
                HttpContext.TraceIdentifier,
                new Dictionary<string, string[]> { ["orderId"] = ["OrderId cannot be empty."] });

            return BadRequest(badRequestResponse);
        }

        var result = await _sender.Send(new GetOrderByIdQuery(orderId), cancellationToken);

        if (result is null)
        {
            var notFoundResponse = ApiResponseFactory.Error(
                "NotFound",
                "Order was not found.",
                StatusCodes.Status404NotFound,
                HttpContext.TraceIdentifier);

            return NotFound(notFoundResponse);
        }

        return Ok(ApiResponseFactory.Success(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Creates a new order and submits it.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "OrdersWrite")]
    [ProducesResponseType(typeof(ApiSuccessResponse<CreateOrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var validationErrors = OrderRequestValidator.ValidateCreateRequest(request);
        if (validationErrors.Count > 0)
        {
            var validationResponse = ApiResponseFactory.Error(
                "ValidationError",
                "One or more validation errors occurred.",
                StatusCodes.Status400BadRequest,
                HttpContext.TraceIdentifier,
                validationErrors);

            return BadRequest(validationResponse);
        }

        var command = new CreateOrderCommand(
            request.CustomerId,
            request.Items.Select(item => new CreateOrderItemRequest(item.ProductId, item.Quantity, item.UnitPrice)).ToList());

        var result = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { orderId = result.OrderId },
            ApiResponseFactory.Success(result, HttpContext.TraceIdentifier, "Order created successfully."));
    }

    /// <summary>
    /// Cancels an order when business rules allow cancellation.
    /// </summary>
    [HttpPost("{orderId:guid}/cancel")]
    [Authorize(Policy = "OrdersWrite")]
    [ProducesResponseType(typeof(ApiSuccessResponse<CancelOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CancelOrderCommand(orderId), cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResponseFactory.Error("NotFound", "Order was not found.", 404, HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponseFactory.Success(result, HttpContext.TraceIdentifier, "Order cancelled successfully."));
    }
}
