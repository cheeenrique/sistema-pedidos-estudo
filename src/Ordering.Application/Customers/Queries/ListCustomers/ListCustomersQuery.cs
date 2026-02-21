using MediatR;
using Ordering.Application.Common.Models;

namespace Ordering.Application.Customers.Queries.ListCustomers;

public sealed record ListCustomersQuery(
    int Page,
    int PageSize,
    string? Search,
    CustomerSortBy SortBy,
    CustomerSortDirection SortDirection) : IRequest<PagedResult<CustomerListItemResponse>>;

public sealed record CustomerListItemResponse(
    Guid CustomerId,
    string FullName,
    string Email,
    string DocumentNumber,
    string PhoneNumber,
    string? CustomerType,
    string? City,
    string? State,
    bool IsActive,
    DateTime CreatedAtUtc);
