using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.Infrastructure.EF.Configuration;

public class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(p => p.Cca2);
        
        builder.Property(x => x.Cca2)
            .IsRequired()
            .HasMaxLength(2);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(TextLengthRules.Name);
    }
}