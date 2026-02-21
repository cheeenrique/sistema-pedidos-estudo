using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Customers.Queries.ListCustomers;
using Ordering.Domain.Customers;

namespace Ordering.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly OrderingDbContext _dbContext;

    public CustomerRepository(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
    }

    public Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return _dbContext.Customers.FirstOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);
    }

    public async Task<(IReadOnlyCollection<Customer> Customers, int TotalCount)> ListPagedAsync(
        int page,
        int pageSize,
        string? search,
        CustomerSortBy sortBy,
        CustomerSortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();
            query = query.Where(customer =>
                customer.FullName.ToLower().Contains(normalizedSearch) ||
                customer.Email.ToLower().Contains(normalizedSearch));
        }

        var isDescending = sortDirection == CustomerSortDirection.Desc;
        query = sortBy switch
        {
            CustomerSortBy.FullName => isDescending
                ? query.OrderByDescending(customer => customer.FullName)
                : query.OrderBy(customer => customer.FullName),
            CustomerSortBy.Email => isDescending
                ? query.OrderByDescending(customer => customer.Email)
                : query.OrderBy(customer => customer.Email),
            _ => isDescending
                ? query.OrderByDescending(customer => customer.CreatedAtUtc)
                : query.OrderBy(customer => customer.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var customers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }
}
