using ECommerce.Api.Application.DTOs.Product;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

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
            Category = product.Category.Slug,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
        };
    }

    public static async Task<Product> GetEntityAsync(this ProductCreateDto dto, ECommerceContext context)
    {
        var category = await FindCategory(dto.Category, context);

        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Sku = dto.Sku,
            Category = category,
            CategoryId = category?.Id,
            ImageUrl = dto.ImageUrl,
            Stock = dto.InitialStock
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
        
        if (dto.Category != null)
        {
            updated.Category = await FindCategory(dto.Category, context);
            updated.CategoryId =  updated.Category?.Id;
        }
        
        return updated;
    }

    private static async Task<Category?> FindCategory(string category, ECommerceContext context)
    {
        if (int.TryParse(category, out var categoryId))
            return await context.Categories.FindAsync(categoryId);
        return await context.Categories.FirstOrDefaultAsync(c => c.Slug == category);
    }
}