using Ordering.Api.Contracts.Orders;
using Ordering.Application.Orders.Queries.ListOrders;
using Ordering.Domain.Orders;

namespace Ordering.Api.Validation;

public static class OrderRequestValidator
{
    public static Dictionary<string, string[]> ValidateCreateRequest(CreateOrderRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.CustomerId == Guid.Empty)
        {
            errors["customerId"] = ["CustomerId cannot be empty."];
        }

        var items = request.Items ?? [];
        if (items.Count == 0)
        {
            errors["items"] = ["At least one item is required."];
            return errors;
        }

        var itemErrors = new List<string>();
        for (var index = 0; index < items.Count; index++)
        {
            var item = items.ElementAt(index);
            if (item.ProductId == Guid.Empty)
            {
                itemErrors.Add($"items[{index}].productId cannot be empty.");
            }

            if (item.Quantity <= 0)
            {
                itemErrors.Add($"items[{index}].quantity must be greater than zero.");
            }

            if (item.UnitPrice < 0)
            {
                itemErrors.Add($"items[{index}].unitPrice cannot be negative.");
            }
        }

        if (itemErrors.Count > 0)
        {
            errors["items"] = itemErrors.ToArray();
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateListRequest(ListOrdersRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (!string.IsNullOrWhiteSpace(request.Status) && !Enum.TryParse<OrderStatus>(request.Status, true, out _))
        {
            errors["status"] = ["Invalid status value."];
        }

        if (request.CreatedFromUtc.HasValue && request.CreatedToUtc.HasValue && request.CreatedFromUtc.Value > request.CreatedToUtc.Value)
        {
            errors["createdRange"] = ["createdFromUtc cannot be greater than createdToUtc."];
        }

        if (!TryParseSortBy(request.SortBy, out _))
        {
            errors["sortBy"] = ["Allowed values: createdAtUtc, customerId, status."];
        }

        if (!TryParseSortDirection(request.SortDirection, out _))
        {
            errors["sortDirection"] = ["Allowed values: asc, desc."];
        }

        return errors;
    }

    public static OrderSortBy ParseSortByOrDefault(string? sortBy)
    {
        if (TryParseSortBy(sortBy, out var parsed))
        {
            return parsed;
        }

        return OrderSortBy.CreatedAtUtc;
    }

    public static SortDirection ParseSortDirectionOrDefault(string? sortDirection)
    {
        if (TryParseSortDirection(sortDirection, out var parsed))
        {
            return parsed;
        }

        return SortDirection.Desc;
    }

    private static bool TryParseSortBy(string? sortBy, out OrderSortBy parsed)
    {
        var normalized = (sortBy ?? string.Empty).Trim().ToLowerInvariant();
        parsed = normalized switch
        {
            "" => OrderSortBy.CreatedAtUtc,
            "createdatutc" => OrderSortBy.CreatedAtUtc,
            "customerid" => OrderSortBy.CustomerId,
            "status" => OrderSortBy.Status,
            _ => OrderSortBy.CreatedAtUtc
        };

        return normalized is "" or "createdatutc" or "customerid" or "status";
    }

    private static bool TryParseSortDirection(string? sortDirection, out SortDirection parsed)
    {
        var normalized = (sortDirection ?? string.Empty).Trim().ToLowerInvariant();
        parsed = normalized switch
        {
            "" => SortDirection.Desc,
            "asc" => SortDirection.Asc,
            "desc" => SortDirection.Desc,
            _ => SortDirection.Desc
        };

        return normalized is "" or "asc" or "desc";
    }
}
