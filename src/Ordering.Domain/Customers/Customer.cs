using Ordering.Domain.Common;

namespace Ordering.Domain.Customers;

public sealed class Customer : Entity
{
    private Customer()
    {
    }

    private Customer(string fullName, string email)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        Email = email;
        CreatedAtUtc = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Customer Create(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("FullName is required.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        return new Customer(fullName.Trim(), email.Trim().ToLowerInvariant());
    }
}
