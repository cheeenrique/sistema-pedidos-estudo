using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Contracts.Customers;

public sealed record CreateCustomerRequest(
    [property: Required] string FullName,
    [property: Required, EmailAddress] string Email);

public sealed record ListCustomersRequest(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = null,
    string? SortDirection = null);
