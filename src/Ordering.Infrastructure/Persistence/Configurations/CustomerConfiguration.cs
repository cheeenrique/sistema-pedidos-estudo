using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Customers;

namespace Ordering.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.FullName).HasMaxLength(200).IsRequired();
        builder.Property(customer => customer.Email).HasMaxLength(200).IsRequired();
        builder.Property(customer => customer.IsActive).IsRequired();
        builder.Property(customer => customer.CreatedAtUtc).IsRequired();

        builder.Ignore(customer => customer.DomainEvents);

        builder.HasIndex(customer => customer.Email).IsUnique();
    }
}
