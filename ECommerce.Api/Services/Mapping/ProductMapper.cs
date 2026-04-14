using ECommerce.Api.DTOs.Product;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.Mapping;

public class ProductMapper(ECommerceContext context)
{
    public ProductResponseDto MapToDto(Product product)
    {
        // if (product.Category == null)
        //     throw new InvalidOperationException($"{nameof(product.Category)} must be included.");

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Sku = product.Sku,
            Category = product.Category?.Slug,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
        };
    }

    public async Task<Product> MapToEntityAsync(ProductCreateDto dto)
    {
        // If the category can't be found, a dummy Category object is used to fail validation
        var category = await FindCategory(dto.Category) ?? new Category();

        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Sku = dto.Sku,
            Category = category,
            CategoryId = category.Id,
            ImageUrl = dto.ImageUrl,
            Stock = dto.InitialStock
        };
    }

    public async Task ApplyUpdateAsync(Product toUpdate, ProductUpdateDto dto)
    {
        // if (toUpdate.Category == null)
        //     throw new InvalidOperationException($"{nameof(toUpdate.Category)} must be included.");

        if (dto.Name != null && dto.Name != toUpdate.Name)
            toUpdate.Name = dto.Name;

        if (dto.Description != null && dto.Description != toUpdate.Description)
            toUpdate.Description = dto.Description;

        if (dto.Price != null && dto.Price != toUpdate.Price)
            toUpdate.Price = dto.Price.Value;

        if (dto.ImageUrl != null && dto.ImageUrl != toUpdate.ImageUrl)
            toUpdate.ImageUrl = dto.ImageUrl;

        if (dto.Category != null && dto.Category != toUpdate.Category?.Slug)
        {
            // If the category can't be found, a dummy Category object is used to fail validation
            var category = await FindCategory(dto.Category) ?? new Category();
            toUpdate.Category = category;
            toUpdate.CategoryId = category.Id;
        }
    }

    private async Task<Category?> FindCategory(string category)
    {
        if (int.TryParse(category, out var categoryId))
            return await context.Categories.FindAsync(categoryId);
        return await context.Categories.FirstOrDefaultAsync(c => c.Slug == category);
    }
}