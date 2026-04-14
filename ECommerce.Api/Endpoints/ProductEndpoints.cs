using ECommerce.Api.DTOs.Product;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.DataAccess;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/products")
            .WithTags("Product")
            .AllowAnonymous();

        group.MapGet("", GetProducts)
            .WithSummary("Get all Products");
        group.MapGet("categories/{category}", GetProductsByCategory)
            .WithSummary("Get Products by Category");
        // group.MapGet("categories", GetCategoriesList)
        //     .WithSummary("Get all Categories slugs");
        group.MapGet("{id:int}", GetProductById)
            .WithSummary("Get Product by Id");

        group.MapPost("", CreateProduct)
            .WithSummary("Create a new Product")
            .RequireAuthorization(UserRoles.Admin);
        group.MapPut("{id:int}", UpdateProduct)
            .WithSummary("Update a Product")
            .RequireAuthorization(UserRoles.Admin);
        group.MapDelete("{id:int}", DeleteProduct)
            .WithSummary("Delete a Product")
            .RequireAuthorization(UserRoles.Admin);

        group.MapPut("{id:int}/restock", RestockProduct)
            .WithSummary("Restock a Product")
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

    private static async Task<Results<Created<ProductResponseDto>, ValidationProblem, UnprocessableEntity<Error>>> CreateProduct(
        HttpContext httpContext,
        IProductService productService,
        ProductCreateDto dto,
        IValidator<ProductCreateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());
        
        var result = await productService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);

        return TypedResults.UnprocessableEntity(result.Error);
    }

    private static async Task<Results<Ok<ProductResponseDto>, ValidationProblem, NotFound, UnprocessableEntity<Error>>> UpdateProduct(
        IProductService productService, 
        int id, 
        ProductUpdateDto dto,
        IValidator<ProductUpdateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());
        
        var result = await productService.UpdateAsync(id, dto);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }

    private static async Task<Results<Ok<ProductResponseDto>, NotFound>> DeleteProduct(
        IProductService productService, int id)
    {
        var deleted = await productService.DeleteAsync(id);
        return deleted != null ? TypedResults.Ok(deleted) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductResponseDto>, NotFound, ValidationProblem, UnprocessableEntity<Error>>> RestockProduct(
        IProductService productService, int id, [FromQuery] int amount)
    {
        var result = await productService.Restock(id, amount);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }
}