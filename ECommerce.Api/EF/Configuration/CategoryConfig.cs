using ECommerce.Api.Entities;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.EF.Configuration;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(TextLengthRules.Name);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(TextLengthRules.Name);
    }
}