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
        endpoints.MapGet("api/categories", GetCategories)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapGet("api/categories/{id:int}", GetCategoryById)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapPost("api/categories", CreateCategory)
            .RequireAuthorization(UserRoles.Admin);
        endpoints.MapPut("api/categories/{id:int}", UpdateCategory)
            .RequireAuthorization(UserRoles.Admin);
        endpoints.MapDelete("api/categories/{id:int}", DeleteCategory)
            .RequireAuthorization(UserRoles.Admin);

        return endpoints;
    }

    private static async Task<Results<Ok<CategoryDto>, ValidationProblem>> CreateCategory(
        ICategoryService categoryService, CategoryCreateDto dto)
    {
        var result = await categoryService.CreateAsync(dto);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.ValidationProblem(result.Error!.Details);
    }

    private static async Task<Results<Ok<CategoryDto>, ValidationProblem, NotFound>> UpdateCategory(
        ICategoryService categoryService, int id, CategoryUpdateDto dto)
    {
        var result = await categoryService.UpdateAsync(id, dto);
        if (result is { IsSuccess: false, Error.ErrorType: ErrorType.NotFound })
            return TypedResults.NotFound();
        
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.ValidationProblem(result.Error!.Details);
    }

    private static async Task<Results<Ok<CategoryDto>, NotFound>> DeleteCategory(
        ICategoryService categoryService, int id)
    {
        var category = await categoryService.DeleteAsync(id);
        if (category == null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(category);
    }

    private static async Task<Results<Ok<CategoryDto>, NotFound>> GetCategoryById(
        ICategoryService categoryService, int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category == null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(category);
    }

    private static async Task<Ok<IEnumerable<CategoryDto>>> GetCategories(
        ICategoryService categoryService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string search = "")
    {
        var categories = await categoryService.GetManyAsync(pagination, search);
        return TypedResults.Ok(categories);
    } 
}