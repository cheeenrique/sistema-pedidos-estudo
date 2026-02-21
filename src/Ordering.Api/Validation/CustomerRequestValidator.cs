using Ordering.Api.Contracts.Customers;
using Ordering.Application.Customers.Queries.ListCustomers;

namespace Ordering.Api.Validation;

public static class CustomerRequestValidator
{
    public static Dictionary<string, string[]> ValidateCreateRequest(CreateCustomerRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            errors["fullName"] = ["FullName is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors["email"] = ["Email is required."];
        }

        if (string.IsNullOrWhiteSpace(request.DocumentNumber))
        {
            errors["documentNumber"] = ["DocumentNumber is required."];
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            errors["phoneNumber"] = ["PhoneNumber is required."];
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerType))
        {
            var normalizedType = request.CustomerType.Trim().ToLowerInvariant();
            if (normalizedType is not ("individual" or "company"))
            {
                errors["customerType"] = ["Allowed values: individual, company."];
            }
        }

        if (request.BirthDate.HasValue && request.BirthDate.Value.Date > DateTime.UtcNow.Date)
        {
            errors["birthDate"] = ["BirthDate cannot be in the future."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> ValidateListRequest(ListCustomersRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (!TryParseSortBy(request.SortBy, out _))
        {
            errors["sortBy"] = ["Allowed values: createdAtUtc, fullName, email."];
        }

        if (!TryParseSortDirection(request.SortDirection, out _))
        {
            errors["sortDirection"] = ["Allowed values: asc, desc."];
        }

        return errors;
    }

    public static CustomerSortBy ParseSortByOrDefault(string? sortBy)
    {
        TryParseSortBy(sortBy, out var parsed);
        return parsed;
    }

    public static CustomerSortDirection ParseSortDirectionOrDefault(string? sortDirection)
    {
        TryParseSortDirection(sortDirection, out var parsed);
        return parsed;
    }

    private static bool TryParseSortBy(string? sortBy, out CustomerSortBy parsed)
    {
        var normalized = (sortBy ?? string.Empty).Trim().ToLowerInvariant();
        parsed = normalized switch
        {
            "" => CustomerSortBy.CreatedAtUtc,
            "createdatutc" => CustomerSortBy.CreatedAtUtc,
            "fullname" => CustomerSortBy.FullName,
            "email" => CustomerSortBy.Email,
            _ => CustomerSortBy.CreatedAtUtc
        };

        return normalized is "" or "createdatutc" or "fullname" or "email";
    }

    private static bool TryParseSortDirection(string? sortDirection, out CustomerSortDirection parsed)
    {
        var normalized = (sortDirection ?? string.Empty).Trim().ToLowerInvariant();
        parsed = normalized switch
        {
            "" => CustomerSortDirection.Desc,
            "asc" => CustomerSortDirection.Asc,
            "desc" => CustomerSortDirection.Desc,
            _ => CustomerSortDirection.Desc
        };

        return normalized is "" or "asc" or "desc";
    }
}
