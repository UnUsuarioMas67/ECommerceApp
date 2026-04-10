using ECommerce.Api.Entities;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.EF.Configuration;

public class OrderStatusConfig : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(TextLengthRules.ShortName);

        builder.HasData(OrderStatuses.GetAll());
    }
}