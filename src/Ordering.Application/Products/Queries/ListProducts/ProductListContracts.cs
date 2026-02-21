namespace Ordering.Application.Products.Queries.ListProducts;

public enum ProductSortBy
{
    CreatedAtUtc = 1,
    Sku = 2,
    Name = 3,
    Price = 4
}

public enum ProductSortDirection
{
    Asc = 1,
    Desc = 2
}
