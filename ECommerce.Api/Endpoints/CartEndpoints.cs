using ECommerce.Api.Application.DTOs.Cart;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Errors;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/carts")
            .WithTags("Cart")
            .RequireAuthorization(UserRoles.Client);

        group.MapGet("{id:int}", GetCartById);
        group.MapGet("", GetCarts);
        group.MapGet("clients/{id:int}", GetCartsByClient);
        
        group.MapPost("", CreateCart);
        group.MapPut("{id:int}", UpdateCart);
        group.MapDelete("{id:int}", DeleteCart);
        
        return endpoints;
    }

    private static async Task<Results<Ok<CartResponseDto>, NotFound>> GetCartById(
        ICartsService cartsService, int id)
    {
        var cart = await cartsService.GetByIdAsync(id);
        return cart != null ? TypedResults.Ok(cart) : TypedResults.NotFound();
    }

    private static async Task<Ok<IEnumerable<CartResponseDto>>> GetCarts(
        ICartsService cartsService, [AsParameters] PaginationQuery pagination)
    {
        var carts = await cartsService.GetManyAsync(pagination);
        return TypedResults.Ok(carts);
    }
    
    private static async Task<Ok<IEnumerable<CartResponseDto>>> GetCartsByClient(
        ICartsService cartsService,
        int id, 
        [AsParameters] PaginationQuery pagination)
    {
        var carts = await cartsService.GetByClientAsync(id, pagination);
        return TypedResults.Ok(carts);
    }

    private static async Task<Results<Created<CartResponseDto>, ValidationProblem, UnprocessableEntity<Error>>> CreateCart(
        HttpContext httpContext,
        ICartsService cartsService,
        CartCreateDto dto,
        IValidator<CartCreateDto> cartValidator)
    {
        var validationResult = await cartValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        
        var result = await cartsService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);

        return TypedResults.UnprocessableEntity(result.Error);
    }
    
    private static async Task<Results<Ok<CartResponseDto>, ValidationProblem, NotFound, UnprocessableEntity<Error>>> UpdateCart(
        ICartsService cartsService,
        int id,
        CartUpdateDto dto,
        IValidator<CartUpdateDto> cartValidator)
    {
        var validationResult = await cartValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        
        var result = await cartsService.UpdateAsync(id, dto);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }
    
    private static async Task<Results<Ok<CartResponseDto>, NotFound>> DeleteCart(
        ICartsService cartsService, int id)
    {
        var deleted = await cartsService.DeleteAsync(id);
        return deleted != null ? TypedResults.Ok(deleted) : TypedResults.NotFound(); 
    }
}