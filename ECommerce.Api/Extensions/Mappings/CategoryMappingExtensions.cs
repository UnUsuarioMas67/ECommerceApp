using ECommerce.Api.Application.DTOs.Category;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Extensions.Mappings;

public static class CategoryMappingExtensions
{
    public static CategoryResponseDto GetDto(this Category category)
        => new()
        {
            Id = category.Id,
            Slug = category.Slug,
            Name = category.Name,
        };

    public static Category GetEntity(this CategoryCreateDto dto)
        => new()
        {
            Slug = dto.Slug,
            Name = dto.Name,
        };

    public static Category GetUpdated(this Category category, CategoryUpdateDto dto)
    {
        var updated = PropertyCopier.GetCopy(category);
        updated.Slug = dto.Slug ?? updated.Slug;
        updated.Name = dto.Name ?? updated.Name;
        return updated;
    }
}