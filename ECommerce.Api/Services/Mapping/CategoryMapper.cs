using ECommerce.Api.DTOs.Category;
using ECommerce.Api.Entities;

namespace ECommerce.Api.Services.Mapping;

public class CategoryMapper
{
    public CategoryResponseDto MapToDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Slug = category.Slug,
            Name = category.Name,
        };
    }

    public Category MapToEntity(CategoryCreateDto dto)
        => new()
        {
            Slug = dto.Slug,
            Name = dto.Name,
        };

    public void ApplyUpdate(Category category, CategoryUpdateDto dto)
    {
        if (dto.Slug != null &&  dto.Slug != category.Slug)
            category.Slug = dto.Slug;
        
        if (dto.Name != null &&  dto.Name != category.Name)
            category.Name = dto.Name;
    }
}