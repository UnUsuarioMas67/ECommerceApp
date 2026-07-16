using ECommerce.Api.DTOs.Category;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Mapping;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.DataAccess;

public interface ICategoryService
{
    Task<CategoryResponseDto?> GetByIdAsync(int categoryId);
    Task<CategoryResponseDto?> GetBySlugAsync(string categorySlug);

    Task<IEnumerable<CategoryResponseDto>> GetManyAsync(string? search = null);
    Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto dto);

    Task<Result<CategoryResponseDto>> UpdateAsync(int categoryId, CategoryUpdateDto dto);
    Task<Result<CategoryResponseDto>> UpdateAsync(string categorySlug, CategoryUpdateDto dto);

    Task<CategoryResponseDto?> DeleteAsync(int categoryId);
    Task<CategoryResponseDto?> DeleteAsync(string categorySlug);
}

public class CategoryService(ECommerceContext context, IValidator<Category> validator, CategoryMapper mapper)
    : ICategoryService
{
    public async Task<CategoryResponseDto?> GetByIdAsync(int categoryId)
    {
        var category = await context.Categories.FindAsync(categoryId);
        return category != null ? mapper.MapToDto(category) : null;
    }

    public async Task<CategoryResponseDto?> GetBySlugAsync(string categorySlug)
    {
        var category = await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == categorySlug);
        return category != null ? mapper.MapToDto(category) : null;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetManyAsync(string? search = null)
    {
        var categories = await context.Categories
            .AsNoTracking()
            .Where(c => c.Name.Contains(search ?? "") || c.Slug.Contains(search ?? ""))
            .Select(category => mapper.MapToDto(category))
            .ToListAsync();

        return categories;
    }

    public async Task<Result<CategoryResponseDto>> CreateAsync(CategoryCreateDto dto)
    {
        var created = mapper.MapToEntity(dto);

        var verification = await VerifyCategory(created);
        if (!verification.IsSuccess)
            return verification.Error;

        await context.Categories.AddAsync(created);
        await context.SaveChangesAsync();

        return mapper.MapToDto(created);
    }


    private async Task<Result<CategoryResponseDto>> UpdateAsync(Category? updated, CategoryUpdateDto dto)
    {
        if (updated == null)
            return new NotFoundError();

        mapper.ApplyUpdate(updated, dto);

        var verification = await VerifyCategory(updated);
        if (!verification.IsSuccess)
        {
            await context.DisposeAsync();
            return verification.Error;
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

    private async Task<Result> VerifyCategory(Category category)
    {
        if (!await SlugIsUnique(category))
            return new DuplicateCategorySlugError(category.Slug, category.Id > 0 ? category.Id : null);

        var validation = await validator.ValidateAsync(category);
        if (!validation.IsValid)
            return new ValidationError(validation.ToDictionary());

        return Result.Success();
    }

    private async Task<bool> SlugIsUnique(Category category)
        => !await context.Categories.AnyAsync(c => c.Slug == category.Slug && c != category);
}