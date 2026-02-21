using Ordering.Domain.Common;

namespace Ordering.Domain.Customers;

public sealed class Customer : Entity
{
    private Customer()
    {
    }

    private Customer(
        string fullName,
        string email,
        string documentNumber,
        string phoneNumber,
        string? customerType,
        DateTime? birthDate,
        string? street,
        string? city,
        string? state,
        string? postalCode,
        string? country,
        string? notes)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
        DocumentNumber = documentNumber;
        PhoneNumber = phoneNumber;
        CustomerType = customerType;
        BirthDate = birthDate;
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        Notes = notes;
        CreatedAtUtc = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? CustomerType { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? Street { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Customer Create(
        string fullName,
        string email,
        string documentNumber,
        string phoneNumber,
        string? customerType,
        DateTime? birthDate,
        string? street,
        string? city,
        string? state,
        string? postalCode,
        string? country,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("FullName is required.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            throw new ArgumentException("DocumentNumber is required.", nameof(documentNumber));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("PhoneNumber is required.", nameof(phoneNumber));
        }

        return new Customer(
            fullName.Trim(),
            email.Trim().ToLowerInvariant(),
            documentNumber.Trim(),
            phoneNumber.Trim(),
            string.IsNullOrWhiteSpace(customerType) ? null : customerType.Trim(),
            birthDate,
            string.IsNullOrWhiteSpace(street) ? null : street.Trim(),
            string.IsNullOrWhiteSpace(city) ? null : city.Trim(),
            string.IsNullOrWhiteSpace(state) ? null : state.Trim(),
            string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim(),
            string.IsNullOrWhiteSpace(country) ? null : country.Trim(),
            string.IsNullOrWhiteSpace(notes) ? null : notes.Trim());
    }
}
