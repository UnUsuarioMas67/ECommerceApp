using System.Security.Claims;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class ClientsEndpoints
{
    public static IEndpointRouteBuilder MapClientsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/clients")
            .RequireAuthorization(UserRoles.Client);

        group.MapGet("me", GetAuthClient);
        group.MapGet("{id:int}", GetClientById);
        group.MapGet("", GetClients);
        group.MapPut("{id:int}", UpdateClient);
        group.MapDelete("{id:int}", DeleteClient);

        group.MapPost("", CreateClient)
            .AllowAnonymous();

        return endpoints;
    }


    private static async Task<Results<Ok<UserResponseDto>, NotFound>> GetClientById(
        IClientsService clientsService,
        int id)
    {
        var clientDto = await clientsService.GetByIdAsync(id);
        return clientDto != null ? TypedResults.Ok(clientDto) : TypedResults.NotFound();
    }


    private static async Task<Results<Ok<UserResponseDto>, NotFound>> GetAuthClient(
        HttpContext context,
        IClientsService clientsService)
    {
        var idClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var success = int.TryParse(idClaim, out var id);
        if (!success)
            return TypedResults.NotFound();

        var clientDto = await clientsService.GetByIdAsync(id);
        return clientDto != null ? TypedResults.Ok(clientDto) : TypedResults.NotFound();
    }


    private static async Task<Ok<IEnumerable<UserResponseDto>>> GetClients(
        IClientsService clientsService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string? search = null)
    {
        var clients = await clientsService.GetManyAsync(pagination, search);
        return TypedResults.Ok(clients);
    }


    private static async Task<Results<Created<UserResponseDto>, ValidationProblem>> CreateClient(
        HttpContext httpContext,
        IClientsService clientsService,
        UserCreateDto dto,
        IValidator<UserCreateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await clientsService.CreateAsync(dto);
        var path = httpContext.Request.Path;
        return result.IsSuccess
            ? TypedResults.Created($"{path}/{result.Value!.Id}", result.Value)
            : TypedResults.ValidationProblem(result.Error!.Details);
    }


    private static async Task<Results<Ok<UserResponseDto>, ValidationProblem, NotFound>> UpdateClient(
        IClientsService clientsService,
        int id,
        UserUpdateDto dto,
        IValidator<UserUpdateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await clientsService.UpdateAsync(id, dto);
        if (result.Error?.ErrorType == ErrorType.NotFound)
            return TypedResults.NotFound();

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.ValidationProblem(result.Error!.Details);
    }


    private static async Task<Results<Ok<UserResponseDto>, NotFound>> DeleteClient(
        IClientsService clientsService,
        int id)
    {
        var client = await clientsService.DeleteAsync(id);
        return client != null ? TypedResults.Ok(client) : TypedResults.NotFound();
    }
}