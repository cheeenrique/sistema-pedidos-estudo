using MediatR;

namespace Ordering.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
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
    string? Notes) : IRequest<CreateCustomerResponse>;

public sealed record CreateCustomerResponse(
    Guid CustomerId,
    string FullName,
    string Email,
    string DocumentNumber,
    string PhoneNumber,
    string? CustomerType,
    DateTime? BirthDate,
    string? City,
    string? State,
    string? Country,
    bool IsActive);
