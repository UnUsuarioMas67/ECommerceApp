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
        endpoints.MapGet("{id:int}", GetCategoryById)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapPost("", CreateCategory)
            .RequireAuthorization(UserRoles.Admin);
        endpoints.MapPut("{id:int}", UpdateCategory)
            .RequireAuthorization(UserRoles.Admin);
        endpoints.MapDelete("{id:int}", DeleteCategory)
            .RequireAuthorization(UserRoles.Admin);

        return endpoints;
    }

    private static async Task<Results<Ok<CategoryResponseDto>, ValidationProblem>> CreateCategory(
        ICategoryService categoryService, CategoryCreateDto dto)
    {
        var result = await categoryService.CreateAsync(dto);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.ValidationProblem(result.Error!.Details);
    }

    private static async Task<Results<Ok<CategoryResponseDto>, ValidationProblem, NotFound>> UpdateCategory(
        ICategoryService categoryService, int id, CategoryUpdateDto dto)
    {
        var result = await categoryService.UpdateAsync(id, dto);
        if (result is { IsSuccess: false, Error.ErrorType: ErrorType.NotFound })
            return TypedResults.NotFound();
        
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.ValidationProblem(result.Error!.Details);
    }

    private static async Task<Results<Ok<CategoryResponseDto>, NotFound>> DeleteCategory(
        ICategoryService categoryService, int id)
    {
        var category = await categoryService.DeleteAsync(id);
        if (category == null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(category);
    }

    private static async Task<Results<Ok<CategoryResponseDto>, NotFound>> GetCategoryById(
        ICategoryService categoryService, int id)
    {
        var category = await categoryService.GetByIdAsync(id);
        if (category == null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(category);
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