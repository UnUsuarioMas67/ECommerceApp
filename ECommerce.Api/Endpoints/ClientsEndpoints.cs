using System.Security.Claims;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Errors;
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
            .WithTags("Client")
            .RequireAuthorization(UserRoles.Client);

        group.MapGet("me", GetAuthClient)
            .WithSummary("Get Authenticated Client");
        group.MapPut("", UpdateClient)
            .WithSummary("Update Authenticated Client");
        group.MapDelete("", DeleteClient)
            .WithSummary("Delete Authenticated Client");

        group.MapGet("{id:int}", GetClientById)
            .WithSummary("Get Client by Id");
        group.MapGet("", GetClients)
            .WithSummary("Get Clients");

        return endpoints;
    }


    private static async Task<Results<Ok<UserResponseDto>, NotFound>> GetClientById(
        IClientsService clientsService,
        int id)
    {
        var clientDto = await clientsService.GetByIdAsync(id);
        return clientDto != null ? TypedResults.Ok(clientDto) : TypedResults.NotFound();
    }


    private static async Task<Results<Ok<UserResponseDto>, BadRequest<InvalidAuthenticationError>>> GetAuthClient(
        HttpContext context,
        IClientsService clientsService)
    {
        var clientId = AuthUser.GetAuthUserId(context);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var clientDto = await clientsService.GetByIdAsync(clientId.Value);
        return clientDto != null
            ? TypedResults.Ok(clientDto)
            : TypedResults.BadRequest(new InvalidAuthenticationError(clientId.Value));
    }


    private static async Task<Ok<IEnumerable<UserResponseDto>>> GetClients(
        IClientsService clientsService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string? search = null)
    {
        var clients = await clientsService.GetManyAsync(pagination, search);
        return TypedResults.Ok(clients);
    }


    private static async Task<Results<Ok<UserResponseDto>, ValidationProblem, BadRequest<InvalidAuthenticationError>,
            UnprocessableEntity<Error>>>
        UpdateClient(
            HttpContext context,
            IClientsService clientsService,
            UserUpdateDto dto,
            IValidator<UserUpdateDto> validator)
    {
        var clientId = AuthUser.GetAuthUserId(context);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await clientsService.UpdateAsync(clientId.Value, dto);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.BadRequest(new InvalidAuthenticationError(clientId.Value)),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }

    private static async Task<Results<Ok<UserResponseDto>, BadRequest<InvalidAuthenticationError>>> DeleteClient(
        HttpContext context,
        IClientsService clientsService)
    {
        var clientId = AuthUser.GetAuthUserId(context);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var client = await clientsService.DeleteAsync(clientId.Value);
        return client != null 
            ? TypedResults.Ok(client) 
            : TypedResults.BadRequest(new InvalidAuthenticationError(clientId.Value));
    }
}