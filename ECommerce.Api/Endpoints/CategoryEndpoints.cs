using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Category;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services;
using ECommerce.Api.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("", GetCategories)
            .WithName("Get categories")
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapGet("{category}", GetCategory)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapPost("", CreateCategory)
            .RequireAuthorization(UserRoles.Admin);
        endpoints.MapPut("{category}", UpdateCategory)
            .RequireAuthorization(UserRoles.Admin);
        endpoints.MapDelete("{category}", DeleteCategory)
            .RequireAuthorization(UserRoles.Admin);

        return endpoints;
    }

    private static async Task<Results<Created<CategoryResponseDto>, ValidationProblem>> CreateCategory(
        ICategoryService categoryService, CategoryCreateDto dto)
    {
        var result = await categoryService.CreateAsync(dto);
        return result.IsSuccess 
            ? TypedResults.Created($"api/categories/{result.Value!.Slug}", result.Value) 
            : TypedResults.ValidationProblem(result.Error!.Details);
    }

    private static async Task<Results<Ok<CategoryResponseDto>, ValidationProblem, NotFound>> UpdateCategory(
        ICategoryService categoryService, string category, CategoryUpdateDto dto)
    {
        Result<CategoryResponseDto> result;
        
        if (int.TryParse(category, out var categoryId))
            result = await categoryService.UpdateAsync(categoryId, dto);
        else
            result = await categoryService.UpdateAsync(category, dto);

        if (result is { IsSuccess: false, Error.ErrorType: ErrorType.NotFound })
            return TypedResults.NotFound();

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.ValidationProblem(result.Error!.Details);
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