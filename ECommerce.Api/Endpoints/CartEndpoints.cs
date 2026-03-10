using ECommerce.Api.Application.DTOs.Cart;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class CartEndpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/carts")
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

    private static async Task<Ok<CartListResponseDto>> GetCarts(
        ICartsService cartsService, [AsParameters] PaginationQuery pagination)
    {
        var carts = await cartsService.GetManyAsync(pagination);

        var list = new CartListResponseDto
        {
            Carts = carts,
            Pagination = pagination,
        };

        return TypedResults.Ok(list);
    }
    
    private static async Task<Ok<CartListResponseDto>> GetCartsByClient(
        ICartsService cartsService,
        IClientsService clientsService,
        int id, 
        [AsParameters] PaginationQuery pagination)
    {
        var client = await clientsService.GetByIdAsync(id);
        var carts = await cartsService.GetByClientAsync(id, pagination);

        var list = new CartListResponseDto
        {
            Client = client,
            Carts = carts,
            Pagination = pagination,
        };

        return TypedResults.Ok(list);
    }

    private static async Task<Results<Ok<CartResponseDto>, ValidationProblem>> CreateCart(
        ICartsService cartsService,
        CartCreateDto dto)
    {
        var result = await cartsService.CreateAsync(dto);
        return result.IsSuccess 
            ? TypedResults.Ok(result.Value) 
            : TypedResults.ValidationProblem(result.Error!.Details);
    }
    
    private static async Task<Results<Ok<CartResponseDto>, ValidationProblem, NotFound>> UpdateCart(
        ICartsService cartsService,
        int id,
        CartUpdateDto dto)
    {
        var result = await cartsService.UpdateAsync(id, dto);
        if (result is { IsSuccess: false, Error.ErrorType: ErrorType.NotFound })
            return TypedResults.NotFound();
        
        return result.IsSuccess 
            ? TypedResults.Ok(result.Value) 
            : TypedResults.ValidationProblem(result.Error!.Details);
    }
    
    private static async Task<Results<Ok<CartResponseDto>, NotFound>> DeleteCart(
        ICartsService cartsService, int id)
    {
        var deleted = await cartsService.DeleteAsync(id);
        return deleted != null ? TypedResults.Ok(deleted) : TypedResults.NotFound(); 
    }
}