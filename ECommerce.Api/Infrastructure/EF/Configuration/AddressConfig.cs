using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.Infrastructure.EF.Configuration;

public class AddressConfig : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(TextLengths.ShortText);
        
        builder.Property(x => x.AddressLine2)
            .HasMaxLength(TextLengths.ShortText);
        
        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(TextLengths.ShortText);
        
        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(TextLengths.PostalCode);
        
        builder.Property(x => x.Region)
            .HasMaxLength(TextLengths.ShortText);
    }
}