using ECommerce.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.Infrastructure.EF.Configuration;

public class OrderStatusConfig : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasData(
            new OrderStatus
            {
                Id = 1,
                Status = "Awaiting Payment"
            },
            new OrderStatus
            {
                Id = 2,
                Status = "In Progress"
            },
            new OrderStatus
            {
                Id = 3,
                Status = "Awaiting Shipping"
            },
            new OrderStatus
            {
                Id = 4,
                Status = "Shipped"
            },
            new OrderStatus
            {
                Id = 5,
                Status = "Out for Delivery"
            },
            new OrderStatus
            {
                Id = 6,
                Status = "Delivered"
            }
        );
    }
}