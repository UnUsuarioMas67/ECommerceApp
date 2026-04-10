using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Api.EF.Configuration;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<decimal> HasMoneyPrecision(this PropertyBuilder<decimal> builder) 
        => builder.HasPrecision(MoneyPrecision.Precision, MoneyPrecision.Scale);
}