using ECommerce.Api.Application.DTOs.Category;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Extensions.Mappings;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services;

public interface ICategoryService
{
    Task<bool> EntryExistsAsync(int categoryId);
    Task<CategoryResponseDto?> GetByIdAsync(int categoryId);
    Task<IEnumerable<CategoryResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null);
    Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto dto);
    Task<Result<CategoryResponseDto>> UpdateAsync(int categoryId, CategoryUpdateDto dto);
    Task<CategoryResponseDto?> DeleteAsync(int categoryId);
}

public class CategoryService(ECommerceContext context, IValidator<Category> validator) : ICategoryService
{
    public async Task<bool> EntryExistsAsync(int categoryId)
        => await context.Categories.AnyAsync(c => c.Id == categoryId);

    public async Task<CategoryResponseDto?> GetByIdAsync(int categoryId)
    {
        var category = await context.Categories.FindAsync(categoryId);
        return category?.GetDto();
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null)
    {
        var categories = await context.Categories.ToListAsync();

        return categories
            .Where(c => c.Name.Contains(search ?? "", StringComparison.InvariantCultureIgnoreCase))
            .Skip(pagination.Skip ?? PaginationDefaults.Skip).Take(pagination.Limit ?? PaginationDefaults.Limit)
            .Select(c => c.GetDto());
    }

    public async Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto dto)
    {
        var created = dto.GetEntity();
        
        var validationResult = await validator.ValidateAsync(created);
        if (!validationResult.IsValid)
            return Errors.ValidationError(validationResult.ToDictionary());
        
        await context.Categories.AddAsync(created);
        await context.SaveChangesAsync();

        return created.GetDto();
    }

    public async Task<Result<CategoryResponseDto>> UpdateAsync(int categoryId, CategoryUpdateDto dto)
    {
        var category = await context.Categories.FindAsync(categoryId);
        if (category == null)
            return Errors.NotFound();

        var updated = category.GetUpdated(dto);
        
        var validationResult = await validator.ValidateAsync(updated);
        if (!validationResult.IsValid)
            return Errors.ValidationError(validationResult.ToDictionary());
        
        PropertyCopier.Mirror(updated, category);
        await context.SaveChangesAsync();
        
        return category.GetDto();
    }

    public async Task<CategoryResponseDto?> DeleteAsync(int categoryId)
    {
        var category = await context.Categories.FindAsync(categoryId);
        if (category == null)
            return null;
        
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        
        return category.GetDto();
    }
}