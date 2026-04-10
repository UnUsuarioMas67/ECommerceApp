using ECommerce.Api.Entities;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.EF.Configuration;

public class AddressConfig : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(TextLengthRules.ShortText);
        
        builder.Property(x => x.AddressLine2)
            .IsRequired()
            .HasMaxLength(TextLengthRules.ShortText);
        
        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(TextLengthRules.ShortText);
        
        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(TextLengthRules.PostalCode);
        
        builder.Property(x => x.Region)
            .IsRequired()
            .HasMaxLength(TextLengthRules.ShortText);
    }
}