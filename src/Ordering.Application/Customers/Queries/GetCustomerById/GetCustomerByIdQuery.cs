using MediatR;

namespace Ordering.Application.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDetailsResponse?>;

public sealed record CustomerDetailsResponse(
    Guid CustomerId,
    string FullName,
    string Email,
    bool IsActive,
    DateTime CreatedAtUtc);
