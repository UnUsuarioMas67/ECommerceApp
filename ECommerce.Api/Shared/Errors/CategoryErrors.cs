namespace ECommerce.Api.Shared.Errors;

public record DuplicateCategorySlugError(string slug, int? categoryId) 
    : Error("A category with this slug already exists");