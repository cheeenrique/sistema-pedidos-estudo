using MediatR;
using Ordering.Application.Abstractions.Persistence;
using Ordering.Application.Common.Models;

namespace Ordering.Application.Customers.Queries.ListCustomers;

public sealed class ListCustomersQueryHandler : IRequestHandler<ListCustomersQuery, PagedResult<CustomerListItemResponse>>
{
    private const int MaxPageSize = 100;
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PagedResult<CustomerListItemResponse>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, MaxPageSize);

        var (customers, totalCount) = await _customerRepository.ListPagedAsync(
            page,
            pageSize,
            request.Search,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var items = customers
            .Select(customer => new CustomerListItemResponse(
                customer.Id,
                customer.FullName,
                customer.Email,
                customer.IsActive,
                customer.CreatedAtUtc))
            .ToList();

        var totalPages = pageSize == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);
        var pagination = new PaginationMetadata(page, pageSize, totalCount, totalPages);

        return new PagedResult<CustomerListItemResponse>(items, pagination);
    }
}
