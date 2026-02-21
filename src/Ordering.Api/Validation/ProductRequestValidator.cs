using Ordering.Api.Contracts.Products;
using Ordering.Application.Products.Queries.ListProducts;

namespace Ordering.Api.Validation;

public static class ProductRequestValidator
{
    public static Dictionary<string, string[]> ValidateCreateRequest(CreateProductRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            errors["sku"] = ["Sku is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors["name"] = ["Name is required."];
        }

        if (request.Price < 0)
        {
            errors["price"] = ["Price cannot be negative."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateListRequest(ListProductsRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (!TryParseSortBy(request.SortBy, out _))
        {
            errors["sortBy"] = ["Allowed values: createdAtUtc, sku, name, price."];
        }

        if (!TryParseSortDirection(request.SortDirection, out _))
        {
            errors["sortDirection"] = ["Allowed values: asc, desc."];
        }

        return errors;
    }

    public static ProductSortBy ParseSortByOrDefault(string? sortBy)
    {
        TryParseSortBy(sortBy, out var parsed);
        return parsed;
    }

    public static ProductSortDirection ParseSortDirectionOrDefault(string? sortDirection)
    {
        TryParseSortDirection(sortDirection, out var parsed);
        return parsed;
    }

    private static bool TryParseSortBy(string? sortBy, out ProductSortBy parsed)
    {
        var normalized = (sortBy ?? string.Empty).Trim().ToLowerInvariant();
        parsed = normalized switch
        {
            "" => ProductSortBy.CreatedAtUtc,
            "createdatutc" => ProductSortBy.CreatedAtUtc,
            "sku" => ProductSortBy.Sku,
            "name" => ProductSortBy.Name,
            "price" => ProductSortBy.Price,
            _ => ProductSortBy.CreatedAtUtc
        };

        return normalized is "" or "createdatutc" or "sku" or "name" or "price";
    }

    private static bool TryParseSortDirection(string? sortDirection, out ProductSortDirection parsed)
    {
        var normalized = (sortDirection ?? string.Empty).Trim().ToLowerInvariant();
        parsed = normalized switch
        {
            "" => ProductSortDirection.Desc,
            "asc" => ProductSortDirection.Asc,
            "desc" => ProductSortDirection.Desc,
            _ => ProductSortDirection.Desc
        };

        return normalized is "" or "asc" or "desc";
    }
}
