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
        builder.Property(customer => customer.DocumentNumber).HasMaxLength(32).IsRequired();
        builder.Property(customer => customer.PhoneNumber).HasMaxLength(32).IsRequired();
        builder.Property(customer => customer.CustomerType).HasMaxLength(32);
        builder.Property(customer => customer.BirthDate);
        builder.Property(customer => customer.Street).HasMaxLength(200);
        builder.Property(customer => customer.City).HasMaxLength(100);
        builder.Property(customer => customer.State).HasMaxLength(64);
        builder.Property(customer => customer.PostalCode).HasMaxLength(32);
        builder.Property(customer => customer.Country).HasMaxLength(64);
        builder.Property(customer => customer.Notes).HasMaxLength(2000);
        builder.Property(customer => customer.IsActive).IsRequired();
        builder.Property(customer => customer.CreatedAtUtc).IsRequired();

        builder.Ignore(customer => customer.DomainEvents);

        builder.HasIndex(customer => customer.Email).IsUnique();
        builder.HasIndex(customer => customer.DocumentNumber).IsUnique();
    }
}
