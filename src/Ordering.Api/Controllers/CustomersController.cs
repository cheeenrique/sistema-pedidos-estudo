using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Api.Common.Responses;
using Ordering.Api.Contracts.Customers;
using Ordering.Api.Validation;
using Ordering.Application.Common.Models;
using Ordering.Application.Customers.Commands.CreateCustomer;
using Ordering.Application.Customers.Queries.GetCustomerById;
using Ordering.Application.Customers.Queries.ListCustomers;

namespace Ordering.Api.Controllers;

/// <summary>
/// Handles customer operations including pagination, creation, and details retrieval.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/customers")]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
public sealed class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lists customers with pagination and sorting options.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "OrdersRead")]
    [ProducesResponseType(typeof(ApiSuccessResponse<PagedResult<CustomerListItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> List([FromQuery] ListCustomersRequest request, CancellationToken cancellationToken)
    {
        var errors = CustomerRequestValidator.ValidateListRequest(request);
        if (errors.Count > 0)
        {
            return BadRequest(ApiResponseFactory.Error("ValidationError", "One or more validation errors occurred.", 400, HttpContext.TraceIdentifier, errors));
        }

        var query = new ListCustomersQuery(
            request.Page,
            request.PageSize,
            request.Search,
            CustomerRequestValidator.ParseSortByOrDefault(request.SortBy),
            CustomerRequestValidator.ParseSortDirectionOrDefault(request.SortDirection));

        var result = await _sender.Send(query, cancellationToken);
        return Ok(ApiResponseFactory.Success(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Gets customer details by identifier.
    /// </summary>
    [HttpGet("{customerId:guid}")]
    [Authorize(Policy = "OrdersRead")]
    [ProducesResponseType(typeof(ApiSuccessResponse<CustomerDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCustomerByIdQuery(customerId), cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResponseFactory.Error("NotFound", "Customer was not found.", 404, HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponseFactory.Success(result, HttpContext.TraceIdentifier));
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CustomerWrite")]
    [ProducesResponseType(typeof(ApiSuccessResponse<CreateCustomerResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var errors = CustomerRequestValidator.ValidateCreateRequest(request);
        if (errors.Count > 0)
        {
            return BadRequest(ApiResponseFactory.Error("ValidationError", "One or more validation errors occurred.", 400, HttpContext.TraceIdentifier, errors));
        }

        var command = new CreateCustomerCommand(
            request.FullName,
            request.Email,
            request.DocumentNumber,
            request.PhoneNumber,
            request.CustomerType,
            request.BirthDate,
            request.Street,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.Notes);
        var result = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { customerId = result.CustomerId }, ApiResponseFactory.Success(result, HttpContext.TraceIdentifier, "Customer created successfully."));
    }
}
