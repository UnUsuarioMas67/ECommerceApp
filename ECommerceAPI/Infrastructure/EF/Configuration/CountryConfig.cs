using ECommerceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceAPI.Infrastructure.EF.Configuration;

public class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        // Add countries
        builder.HasData(new Country
        {
            Id = 1,
            Name = "Dominican Republic"
        });
    }
}