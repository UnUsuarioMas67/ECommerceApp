using ECommerce.Api.Application.DTOs.Category;
using ECommerce.Api.Application.DTOs.Product;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/products")
            .RequireAuthorization(UserRoles.Client);

        group.MapGet("", GetProducts);
        group.MapGet("categories/{category}", GetProductsByCategory);
        group.MapGet("categories", GetCategoriesList);
        group.MapGet("{id:int}", GetProductById);

        group.MapPost("", CreateProduct)
            .RequireAuthorization(UserRoles.Admin);
        group.MapPut("{id:int}", UpdateProduct)
            .RequireAuthorization(UserRoles.Admin);
        group.MapDelete("{id:int}", DeleteProduct)
            .RequireAuthorization(UserRoles.Admin);

        group.MapPut("{id:int}/restock", RestockProduct)
            .RequireAuthorization(UserRoles.Admin);

        return endpoints;
    }

    private static async Task<Ok<IEnumerable<ProductResponseDto>>> GetProducts(
        IProductService productService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string? search)
    {
        var products = await productService.GetManyAsync(pagination, search);
        return TypedResults.Ok(products);
    }

    private static async Task<Ok<IEnumerable<ProductResponseDto>>> GetProductsByCategory(
        IProductService productService,
        [FromRoute] string category,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string? search)
    {
        IEnumerable<ProductResponseDto> products;

        if (int.TryParse(category, out var categoryId))
            products = await productService.GetByCategoryId(categoryId, pagination, search);
        else
            products = await productService.GetByCategorySlug(category, pagination, search);


        return TypedResults.Ok(products);
    }

    private static async Task<Ok<IEnumerable<string>>> GetCategoriesList(
        ICategoryService categoryService, [AsParameters] PaginationQuery pagination)
    {
        var categories = await categoryService.GetManyAsync(pagination);
        return TypedResults.Ok(categories.Select(c => c.Slug));
    }

    private static async Task<Results<Ok<ProductResponseDto>, NotFound>> GetProductById(
        IProductService productService, int id)
    {
        var product = await productService.GetByIdAsync(id);
        return product != null ? TypedResults.Ok(product) : TypedResults.NotFound();
    }

    private static async Task<Results<Created<ProductResponseDto>, ValidationProblem>> CreateProduct(
        HttpContext httpContext,
        IProductService productService,
        ProductCreateDto dto)
    {
        var result = await productService.CreateAsync(dto);
        var created = result.Value!;
        var path = httpContext.Request.Path;
        return result.IsSuccess
            ? TypedResults.Created($"{path}/{created.Id}", created)
            : TypedResults.ValidationProblem(result.Error!.Details);
    }

    private static async Task<Results<Ok<ProductResponseDto>, ValidationProblem, NotFound>> UpdateProduct(
        IProductService productService, int id, ProductUpdateDto dto)
    {
        var result = await productService.UpdateAsync(id, dto);
        if (result is { IsSuccess: false, Error.ErrorType: ErrorType.NotFound })
            return TypedResults.NotFound();

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.ValidationProblem(result.Error!.Details);
    }

    private static async Task<Results<Ok<ProductResponseDto>, NotFound>> DeleteProduct(
        IProductService productService, int id)
    {
        var deleted = await productService.DeleteAsync(id);
        return deleted != null ? TypedResults.Ok(deleted) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductResponseDto>, NotFound, ValidationProblem>> RestockProduct(
        IProductService productService, int id, [FromQuery] int amount)
    {
        var result = await productService.Restock(id, amount);
        if (result is { IsSuccess: false, Error.ErrorType: ErrorType.NotFound })
            return TypedResults.NotFound();

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.ValidationProblem(result.Error!.Details);
    }
}