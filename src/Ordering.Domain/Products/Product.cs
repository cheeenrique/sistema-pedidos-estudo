using Ordering.Domain.Common;

namespace Ordering.Domain.Products;

public sealed class Product : Entity
{
    private Product()
    {
    }

    private Product(string sku, string name, decimal price)
    {
        Id = Guid.NewGuid();
        Sku = sku;
        Name = name;
        Price = price;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Product Create(string sku, string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("Sku is required.", nameof(sku));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        }

        return new Product(sku.Trim().ToUpperInvariant(), name.Trim(), price);
    }
}
