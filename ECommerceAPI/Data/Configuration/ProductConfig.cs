using ECommerceAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceAPI.Data.Configuration;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Stock)
            .IsRequired();
    }
}