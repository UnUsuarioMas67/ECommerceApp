using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record DuplicateCategorySlugError(string Slug, int? CategoryId) 
    : Error("A category with this slug already exists", "category_slug_exists")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["slug"] = Slug,
            ["categoryId"] = CategoryId
        }
    };
}