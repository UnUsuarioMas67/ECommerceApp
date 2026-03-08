using ECommerce.Api.Application.DTOs.Product;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface IProductService
{
    Task<bool> EntryExistsAsync(int productId);
    Task<ProductResponseDto?> GetByIdAsync(int productId);
    Task<Result<ProductResponseDto>> CreateAsync(ProductCreateDto dto);
    Task<Result<ProductResponseDto>> UpdateAsync(int productId, ProductUpdateDto dto);
    Task<ProductResponseDto?> DeleteAsync(int productId);

    Task<IEnumerable<ProductResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null);
    Task<IEnumerable<ProductResponseDto>> GetByCategoryId(int categoryId, PaginationQuery pagination,
        string? search = null);
    Task<IEnumerable<ProductResponseDto>> GetByCategorySlug(string categorySlug, PaginationQuery pagination,
        string? search = null);
    
    Task<Result<ProductResponseDto>> Restock(int productId, int newStock);
}

public class ProductService(ECommerceContext context, IValidator<Product> validator, IProductMapper mapper) 
    : IProductService
{
    public async Task<bool> EntryExistsAsync(int productId)
        => await context.Products.FindAsync(productId) != null;

    public async Task<ProductResponseDto?> GetByIdAsync(int productId)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);
        return product != null ? mapper.ToDto(product) : null;
    }

    public async Task<Result<ProductResponseDto>> CreateAsync(ProductCreateDto dto)
    {
        var created = await mapper.ToEntityAsync(dto, context);

        var validationResult = await validator.ValidateAsync(created);
        if (!validationResult.IsValid)
            return Errors.ValidationError(validationResult.ToDictionary());

        await context.Products.AddAsync(created);
        await context.SaveChangesAsync();

        return mapper.ToDto(created);
    }

    public async Task<Result<ProductResponseDto>> UpdateAsync(int productId, ProductUpdateDto dto)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return Errors.NotFound();

        var updated = await mapper.GetUpdatedEntityAsync(product, dto, context);

        var validationResult = await validator.ValidateAsync(updated);
        if (!validationResult.IsValid)
            return Errors.ValidationError(validationResult.ToDictionary());

        PropertyCopier.Mirror(updated, product);
        await context.SaveChangesAsync();

        return mapper.ToDto(product);
    }

    public async Task<ProductResponseDto?> DeleteAsync(int productId)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return null;

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return mapper.ToDto(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null)
    {
        var products = await context.Products
            .Include(p => p.Category)
            .Where(p => p.Category!.Name.Contains(search ?? ""))
            .Skip(pagination.Skip ?? PaginationDefaults.Skip).Take(pagination.Limit ?? PaginationDefaults.Limit)
            .Select(product => mapper.ToDto(product))
            .ToListAsync();

        return products;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetByCategoryId(int categoryId, PaginationQuery pagination,
        string? search = null)
    {
        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(search ?? "") && p.Category!.Id == categoryId)
            .Skip(pagination.Skip ?? PaginationDefaults.Skip).Take(pagination.Limit ?? PaginationDefaults.Limit)
            .Select(product => mapper.ToDto(product))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetByCategorySlug(string categorySlug, 
        PaginationQuery pagination, string? search = null)
    {
        return await context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(search ?? "") && p.Category!.Slug == categorySlug)
            .Skip(pagination.Skip ?? PaginationDefaults.Skip).Take(pagination.Limit ?? PaginationDefaults.Limit)
            .Select(product => mapper.ToDto(product))
            .ToListAsync(); 
    }

    public async Task<Result<ProductResponseDto>> Restock(int productId, int newStock)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);
        
        if (product == null)
            return Errors.NotFound();

        product.Stock = newStock;
        
        var validation = await validator.ValidateAsync(product);
        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());
        
        return mapper.ToDto(product);
    }
}