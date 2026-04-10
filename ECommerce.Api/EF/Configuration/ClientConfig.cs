using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.EF.Configuration;

public class ClientConfig : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(TextLengthRules.Name);
        
        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(TextLengthRules.Name);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(TextLengthRules.Email);
        
        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(TextLengthRules.PasswordHash);
        
        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(TextLengthRules.PhoneNumber);

        builder.Property(x => x.BirthDate)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}