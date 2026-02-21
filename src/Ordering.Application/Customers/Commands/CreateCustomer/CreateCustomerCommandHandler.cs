using MediatR;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Domain.Customers;

namespace Ordering.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(
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

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCustomerResponse(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.DocumentNumber,
            customer.PhoneNumber,
            customer.CustomerType,
            customer.BirthDate,
            customer.City,
            customer.State,
            customer.Country,
            customer.IsActive);
    }
}
