using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Contracts.Customers;

public sealed class CreateCustomerRequest
{
    [Required]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string DocumentNumber { get; init; } = string.Empty;

    [Required]
    public string PhoneNumber { get; init; } = string.Empty;

    public string? CustomerType { get; init; }
    public DateTime? BirthDate { get; init; }
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string? Notes { get; init; }
}

public sealed class ListCustomersRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}
