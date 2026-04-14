using ECommerce.Api.DTOs.Category;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.DataAccess;
using ECommerce.Api.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/categories")
            .WithTags("Category")
            .RequireAuthorization(UserRoles.Admin);

        group.MapGet("", GetCategories)
            .WithSummary("Get Categories")
            .AllowAnonymous();
        group.MapGet("{category}", GetCategory)
            .WithSummary("Get Single Category")
            .AllowAnonymous();
        group.MapPost("", AddCategory)
            .WithSummary("Add New Category");
        group.MapPut("{category}", UpdateCategory)
            .WithSummary("Update Category");
        group.MapDelete("{category}", DeleteCategory)
            .WithSummary("Delete Category");

        return endpoints;
    }

    private static async Task<Results<Created<CategoryResponseDto>, ValidationProblem, UnprocessableEntity<Error>>>
        AddCategory(HttpContext httpContext, ICategoryService categoryService, CategoryCreateDto dto)
    {
        var result = await categoryService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);

        return TypedResults.UnprocessableEntity(result.Error);
    }

    private static async Task<Results<Ok<CategoryResponseDto>, ValidationProblem, NotFound, UnprocessableEntity<Error>>>
        UpdateCategory(ICategoryService categoryService, string category, CategoryUpdateDto dto)
    {
        Result<CategoryResponseDto> result;

        if (int.TryParse(category, out var categoryId))
            result = await categoryService.UpdateAsync(categoryId, dto);
        else
            result = await categoryService.UpdateAsync(category, dto);

        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }

    private static async Task<Results<Ok<CategoryResponseDto>, NotFound>> DeleteCategory(
        ICategoryService categoryService, string category)
    {
        CategoryResponseDto? deleted;

        if (int.TryParse(category, out var categoryId))
            deleted = await categoryService.DeleteAsync(categoryId);
        else
            deleted = await categoryService.DeleteAsync(category);

        if (deleted == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(deleted);
    }

    private static async Task<Results<Ok<CategoryResponseDto>, NotFound>> GetCategory(
        ICategoryService categoryService, string category)
    {
        CategoryResponseDto? dto;

        if (int.TryParse(category, out var categoryId))
            dto = await categoryService.GetByIdAsync(categoryId);
        else
            dto = await categoryService.GetBySlugAsync(category);

        if (dto == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(dto);
    }

    private static async Task<Ok<IEnumerable<CategoryResponseDto>>> GetCategories(
        ICategoryService categoryService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string search = "")
    {
        var categories = await categoryService.GetManyAsync(pagination, search);
        return TypedResults.Ok(categories);
    }
}