using ECommerce.Api.Application.DTOs.Category;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

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

public class CategoryService(ECommerceContext context, IValidator<Category> validator, CategoryMapper mapper) 
    : ICategoryService
{
    public async Task<bool> EntryExistsAsync(int categoryId)
        => await context.Categories.AnyAsync(c => c.Id == categoryId);

    public async Task<bool> EntryExistsAsync(string categorySlug)
        => await context.Categories.AnyAsync(c => c.Slug == categorySlug);

    public async Task<CategoryResponseDto?> GetByIdAsync(int categoryId)
    {
        var category = await context.Categories.FindAsync(categoryId);
        return category != null ? mapper.MapToDto(category) : null;
    }

    public async Task<CategoryResponseDto?> GetBySlugAsync(string categorySlug)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlug);
        return category != null ? mapper.MapToDto(category) : null;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null)
    {
        var categories = await context.Categories.ToListAsync();

        return categories
            .Where(c => c.Name.Contains(search ?? "", StringComparison.InvariantCultureIgnoreCase))
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1)).Take(pagination.LimitOrDefault)
            .Select(mapper.MapToDto);
    }

    public async Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto dto)
    {
        var created = mapper.MapToEntity(dto);

        var validation = await validator.ValidateAsync(created);
        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());

        await context.Categories.AddAsync(created);
        await context.SaveChangesAsync();

        return mapper.MapToDto(created);
    }
    

    private async Task<Result<CategoryResponseDto>> UpdateAsync(Category? updated, CategoryUpdateDto dto)
    {
        if (updated == null)
            return Errors.NotFound();

        mapper.ApplyUpdate(updated, dto);

        var validation = await validator.ValidateAsync(updated);
        if (!validation.IsValid)
        {
            await context.DisposeAsync();
            return Errors.ValidationError(validation.ToDictionary());
        }

        await context.SaveChangesAsync();

        return mapper.MapToDto(updated);
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

        return mapper.MapToDto(category);
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