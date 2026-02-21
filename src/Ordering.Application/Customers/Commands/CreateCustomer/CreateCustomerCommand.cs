using MediatR;

namespace Ordering.Application.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(string FullName, string Email) : IRequest<CreateCustomerResponse>;

public sealed record CreateCustomerResponse(Guid CustomerId, string FullName, string Email, bool IsActive);
