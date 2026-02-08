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
            .HasMaxLength(TextLengthRules.ShortText);
        
        builder.Property(x => x.AddressLine2)
            .HasMaxLength(TextLengthRules.ShortText);
        
        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(TextLengthRules.ShortText);
        
        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(TextLengthRules.PostalCode);
        
        builder.Property(x => x.Region)
            .HasMaxLength(TextLengthRules.ShortText);
    }
}