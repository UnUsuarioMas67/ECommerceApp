using ECommerceAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceAPI.Infrastructure.EF.Configuration;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(320);
        
        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(25);

        builder.Property(x => x.BirthDate)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}