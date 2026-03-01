using ECommerce.Api.Application.DTOs.Product;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Extensions.Mappings;

public static class ProductMappingExtensions
{
    public static ProductResponseDto GetDto(this Product product)
    {
        if (product.Category == null)
            throw new InvalidOperationException($"{nameof(product.Category)} must not be null.");

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Sku = product.Sku,
            Category = product.Category.Name,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
        };
    }

    public static async Task<Product> GetEntityAsync(this ProductCreateDto dto, ECommerceContext context)
    {
        var category = await context.Categories.FindAsync(dto.CategoryId);

        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Sku = dto.Sku,
            Category = category,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            Stock = 0
        };
    }

    public static async Task<Product> GetUpdatedAsync(
        this Product product, 
        ProductUpdateDto dto,
        ECommerceContext context)
    {
        if (product.Category == null)
            throw new InvalidOperationException($"{nameof(product.Category)} must not be null.");

        var updated = PropertyCopier.GetCopy(product);
        updated.Name = dto.Name ?? updated.Name;
        updated.Description = dto.Description ?? updated.Description;
        updated.Price = dto.Price ?? updated.Price;
        updated.ImageUrl = dto.ImageUrl ?? updated.ImageUrl;
        
        if (dto.CategoryId != null)
        {
            updated.CategoryId =  dto.CategoryId;
            updated.Category = await context.Categories.FindAsync(dto.CategoryId);
        }
        
        return updated;
    }
}