using ECommerce.Api.DTOs.Order;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.DataAccess;
using ECommerce.Api.Shared;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var clientGroup = endpoints.MapGroup("api/orders")
            .WithTags("Orders")
            .RequireAuthorization(UserRoles.Client);

        clientGroup.MapGet("me", GetAuthClientOrders)
            .WithSummary("Get authenticated client orders");
        clientGroup.MapGet("me/{id:int}", GetAuthClientOrderById)
            .WithSummary("Get authenticated client order by id");

        var adminGroup = endpoints.MapGroup("api/orders")
            .WithTags("Orders")
            .RequireAuthorization(UserRoles.Admin);

        adminGroup.MapGet("", GetOrders)
            .WithSummary("Get all orders");
        adminGroup.MapGet("{id:int}", GetOrderById)
            .WithSummary("Get order by id");
        adminGroup.MapGet("clients/{clientId:int}", GetOrdersByClient)
            .WithSummary("Get orders by client id");
        adminGroup.MapGet("products/{productId:int}", GetOrdersByProduct)
            .WithSummary("Get orders by product id");

        return endpoints;
    }

    private static async Task<Results<Ok<IEnumerable<OrderResponseDto>>, UnauthorizedHttpResult>> GetAuthClientOrders(
        HttpContext httpContext,
        IOrderService orderService,
        [AsParameters] PaginationQuery pagination)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.Unauthorized();

        var orders = await orderService.GetByClientAsync(clientId.Value, pagination);
        return TypedResults.Ok(orders);
    }

    private static async Task<Results<Ok<OrderResponseDto>, NotFound, UnauthorizedHttpResult>> GetAuthClientOrderById(
        HttpContext httpContext,
        IOrderService orderService,
        int id)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.Unauthorized();

        var order = await orderService.GetByIdAsync(id, clientId);
        return order != null ? TypedResults.Ok(order) : TypedResults.NotFound();
    }

    private static async Task<Ok<IEnumerable<OrderResponseDto>>> GetOrders(
        IOrderService orderService,
        [AsParameters] PaginationQuery pagination)
    {
        var orders = await orderService.GetManyAsync(pagination);
        return TypedResults.Ok(orders);
    }

    private static async Task<Results<Ok<OrderResponseDto>, NotFound>> GetOrderById(
        IOrderService orderService,
        int id)
    {
        var order = await orderService.GetByIdAsync(id);
        return order != null ? TypedResults.Ok(order) : TypedResults.NotFound();
    }

    private static async Task<Ok<IEnumerable<OrderResponseDto>>> GetOrdersByClient(
        IOrderService orderService,
        int clientId,
        [AsParameters] PaginationQuery pagination)
    {
        var orders = await orderService.GetByClientAsync(clientId, pagination);
        return TypedResults.Ok(orders);
    }

    private static async Task<Ok<IEnumerable<OrderResponseDto>>> GetOrdersByProduct(
        IOrderService orderService,
        int productId,
        [AsParameters] PaginationQuery pagination)
    {
        var orders = await orderService.GetByProductAsync(productId, pagination);
        return TypedResults.Ok(orders);
    }
}