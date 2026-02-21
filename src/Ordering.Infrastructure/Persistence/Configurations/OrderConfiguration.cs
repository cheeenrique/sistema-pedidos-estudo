using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Orders;

namespace Ordering.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(order => order.Id);
        builder.Property(order => order.CustomerId).IsRequired();
        builder.Property(order => order.CreatedAtUtc).IsRequired();
        builder.Property(order => order.Status).IsRequired();

        builder.Ignore(order => order.DomainEvents);
        builder.Ignore(order => order.TotalAmount);

        builder
            .HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Items).AutoInclude();
    }
}
