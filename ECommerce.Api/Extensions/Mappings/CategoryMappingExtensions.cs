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
            Name = category.Name,
            Description = category.Description,
        };

    public static Category GetEntity(this CategoryCreateDto dto)
        => new()
        {
            Name = dto.Name,
            Description = dto.Description ?? "",
        };

    public static Category GetUpdated(this Category category, CategoryUpdateDto dto)
    {
        var updated = PropertyCopier.GetCopy(category);
        updated.Name = dto.Name ?? updated.Name;
        updated.Description = dto.Description ?? updated.Description;
        return updated;
    }
}