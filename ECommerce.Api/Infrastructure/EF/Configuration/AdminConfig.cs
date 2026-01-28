using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.Infrastructure.EF.Configuration;

public class AdminConfig : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(TextLengths.Name);
        
        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(TextLengths.Name);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(TextLengths.Email);
        
        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(TextLengths.PasswordHash);
        
        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(TextLengths.PhoneNumber);

        builder.Property(x => x.BirthDate)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}