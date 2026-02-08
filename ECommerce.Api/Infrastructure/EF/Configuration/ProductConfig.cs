using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.Infrastructure.EF.Configuration;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(TextLengthRules.Sku);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(TextLengthRules.LongText);

        builder.Property(x => x.Price)
            .IsRequired()
            .HasMoneyPrecision();

        builder.Property(x => x.Stock)
            .IsRequired();
    }
}