using MediatR;

namespace Ordering.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDetailsResponse?>;

public sealed record CustomerDetailsResponse(
    Guid CustomerId,
    string FullName,
    string Email,
    string DocumentNumber,
    string PhoneNumber,
    string? CustomerType,
    DateTime? BirthDate,
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? Notes,
    bool IsActive,
    DateTime CreatedAtUtc);
