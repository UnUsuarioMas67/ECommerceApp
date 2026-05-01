using ECommerce.Api.DTOs.Cart;
using ECommerce.Api.DTOs.Error;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.DataAccess;
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

        group.MapGet("{id:int}", GetCartById)
            .WithSummary("Get Cart by Id")
            .RequireAuthorization(o => o.RequireRole(UserRoles.Client, UserRoles.Admin));
        group.MapGet("clients/{id:int}", GetCartsByClient)
            .WithSummary("Get Carts by Client Id")
            .RequireAuthorization(o => o.RequireRole(UserRoles.Client, UserRoles.Admin));
        
        group.MapGet("clients/me", GetAuthClientCarts)
            .WithSummary("Get authenticated Client's Carts");
        group.MapPost("", AddCart)
            .WithSummary("Add cart to authenticated Client");
        group.MapPut("{id:int}", UpdateCart)
            .WithSummary("Update authenticated Client's Carts");
        group.MapDelete("{id:int}", DeleteCart)
            .WithSummary("Delete authenticated Client's Carts");

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

    private static async Task<Results<Ok<IEnumerable<CartResponseDto>>, BadRequest<InvalidAuthenticationError>>>
        GetAuthClientCarts(
            HttpContext httpContext,
            ICartsService cartsService,
            [AsParameters] PaginationQuery pagination)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var carts = await cartsService.GetByClientAsync(clientId.Value, pagination);
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

    private static async Task<Results<Created<CartResponseDto>, ValidationProblem, UnprocessableEntity<ErrorDto>,
            BadRequest<InvalidAuthenticationError>>>
        AddCart(
            HttpContext httpContext,
            ICartsService cartsService,
            CartRequestDto dto,
            IValidator<CartRequestDto> cartValidator)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var validationResult = await cartValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await cartsService.CreateAsync(dto, clientId.Value);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);

        return TypedResults.UnprocessableEntity(result.Error.ToDto());
    }

    private static async Task<Results<Ok<CartResponseDto>, ValidationProblem, NotFound,
        BadRequest<InvalidAuthenticationError>, UnprocessableEntity<ErrorDto>>> UpdateCart(
        HttpContext httpContext,
        ICartsService cartsService,
        int id,
        CartRequestDto dto,
        IValidator<CartRequestDto> cartValidator)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var validationResult = await cartValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await cartsService.UpdateAsync(id, dto, clientId);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error.ToDto())
        };
    }

    private static async Task<Results<Ok<CartResponseDto>, NotFound, BadRequest<InvalidAuthenticationError>>>
        DeleteCart(
            HttpContext httpContext,
            ICartsService cartsService, int id)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var deleted = await cartsService.DeleteAsync(id, clientId);
        return deleted != null ? TypedResults.Ok(deleted) : TypedResults.NotFound();
    }
}