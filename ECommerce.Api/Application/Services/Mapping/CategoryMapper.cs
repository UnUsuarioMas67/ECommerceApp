using ECommerce.Api.Application.DTOs.Category;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Application.Services.Mapping;

public interface ICategoryMapper
    : IEntityDtoMapper<Category, CategoryResponseDto, CategoryCreateDto, CategoryUpdateDto>;

public class CategoryMapper : ICategoryMapper
{
    public CategoryResponseDto ToDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Slug = category.Slug,
            Name = category.Name,
        };
    }

    public Category ToEntity(CategoryCreateDto dto)
        => new()
        {
            Slug = dto.Slug,
            Name = dto.Name,
        };

    public void ApplyUpdateToEntity(Category category, CategoryUpdateDto dto)
    {
        if (dto.Slug != null &&  dto.Slug != category.Slug)
            category.Slug = dto.Slug;
        
        if (dto.Name != null &&  dto.Name != category.Name)
            category.Name = dto.Name;
    }
}