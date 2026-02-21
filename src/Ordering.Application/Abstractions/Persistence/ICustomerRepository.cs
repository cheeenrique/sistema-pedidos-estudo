using Ordering.Application.Customers.Queries.ListCustomers;
using Ordering.Domain.Customers;

namespace Ordering.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<(IReadOnlyCollection<Customer> Customers, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        string? search,
        CustomerSortBy sortBy,
        CustomerSortDirection sortDirection,
        CancellationToken cancellationToken);
}
