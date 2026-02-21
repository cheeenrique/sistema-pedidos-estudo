namespace Ordering.Application.Customers.Queries.ListCustomers;

public enum CustomerSortBy
{
    CreatedAtUtc = 1,
    FullName = 2,
    Email = 3
}

public enum CustomerSortDirection
{
    Asc = 1,
    Desc = 2
}
