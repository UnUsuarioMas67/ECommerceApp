using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.Infrastructure.EF.Configuration;

public class ClientReviewConfig : IEntityTypeConfiguration<ClientReview>
{
    public void Configure(EntityTypeBuilder<ClientReview> builder)
    {
        builder.Property(x => x.Comment)
            .IsRequired()
            .HasMaxLength(TextLengthRules.LongText);

        builder.Property(x => x.Rating)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}