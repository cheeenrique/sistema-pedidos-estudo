using MediatR;
using Ordering.Application.Abstractions.Persistence;

namespace Ordering.Application.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsResponse?>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDetailsResponse?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        return new CustomerDetailsResponse(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.DocumentNumber,
            customer.PhoneNumber,
            customer.CustomerType,
            customer.BirthDate,
            customer.Street,
            customer.City,
            customer.State,
            customer.PostalCode,
            customer.Country,
            customer.Notes,
            customer.IsActive,
            customer.CreatedAtUtc);
    }
}
