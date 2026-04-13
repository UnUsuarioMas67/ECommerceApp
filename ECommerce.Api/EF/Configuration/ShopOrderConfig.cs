using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.EF.Configuration;

public class ShopOrderConfig : IEntityTypeConfiguration<ShopOrder>
{
    public void Configure(EntityTypeBuilder<ShopOrder> builder)
    {
        builder.Property(e => e.StripeSessionId)
            .HasMaxLength(100);
        
        builder.HasIndex(p => p.StripeSessionId).IsUnique();
    }
}