using ECommerce.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.Infrastructure.EF.Configuration;

public class AddressConfig : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.AddressLine2)
            .HasMaxLength(100);
        
        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(x => x.Region)
            .HasMaxLength(50);
    }
}