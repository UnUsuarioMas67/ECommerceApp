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
    Task<bool> EntryExistsAsync(string categorySlug);
    
    Task<CategoryResponseDto?> GetByIdAsync(int categoryId);
    Task<CategoryResponseDto?> GetBySlugAsync(string categorySlug);
    
    Task<IEnumerable<CategoryResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null);
    Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto dto);
    
    Task<Result<CategoryResponseDto>> UpdateAsync(int categoryId, CategoryUpdateDto dto);
    Task<Result<CategoryResponseDto>> UpdateAsync(string categorySlug, CategoryUpdateDto dto);
    
    Task<CategoryResponseDto?> DeleteAsync(int categoryId);
    Task<CategoryResponseDto?> DeleteAsync(string categorySlug);
}

public class CategoryService(ECommerceContext context, IValidator<Category> validator) : ICategoryService
{
    public async Task<bool> EntryExistsAsync(int categoryId)
        => await context.Categories.AnyAsync(c => c.Id == categoryId);

    public async Task<bool> EntryExistsAsync(string categorySlug)
        => await context.Categories.AnyAsync(c => c.Slug == categorySlug);

    public async Task<CategoryResponseDto?> GetByIdAsync(int categoryId)
    {
        var category = await context.Categories.FindAsync(categoryId);
        return category?.GetDto();
    }

    public async Task<CategoryResponseDto?> GetBySlugAsync(string categorySlug)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlug);
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

        var validationResult = await Validate(created);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        await context.Categories.AddAsync(created);
        await context.SaveChangesAsync();

        return created.GetDto();
    }
    

    private async Task<Result<CategoryResponseDto>> UpdateAsync(Category? category, CategoryUpdateDto dto)
    {
        if (category == null)
            return Errors.NotFound();

        var updated = category.GetUpdated(dto);

        var validationResult = await Validate(updated);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        PropertyCopier.Mirror(updated, category);
        await context.SaveChangesAsync();

        return category.GetDto();
    }

    public async Task<Result<CategoryResponseDto>> UpdateAsync(int categoryId, CategoryUpdateDto dto)
    {
        var category = await context.Categories.FindAsync(categoryId);
        return await UpdateAsync(category, dto);
    }

    public async Task<Result<CategoryResponseDto>> UpdateAsync(string categorySlug, CategoryUpdateDto dto)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlug);
        return await UpdateAsync(category, dto);
    }
    

    private async Task<CategoryResponseDto?> DeleteAsync(Category? category)
    {
        if (category == null)
            return null;

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return category.GetDto();
    }

    public async Task<CategoryResponseDto?> DeleteAsync(int categoryId)
    {
        var category = await context.Categories.FindAsync(categoryId);
        return await DeleteAsync(category);
    }

    public async Task<CategoryResponseDto?> DeleteAsync(string categorySlug)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlug);
        return await DeleteAsync(category);
    }


    private async Task<Result> Validate(Category category)
    {
        var validation = await validator.ValidateAsync(category);
        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());

        var slugIsDuplicate = await context.Categories.AnyAsync(c 
            => c.Slug == category.Slug && c.Id != category.Id);
        
        return slugIsDuplicate 
            ? Errors.ValidationError(nameof(category.Slug), "A category with the same slug already exists.") 
            : Result.Success();
    }
}