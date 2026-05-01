using System.Security.Claims;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.DTOs.Error;
using ECommerce.Api.DTOs.User;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.DataAccess;
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
            : TypedResults.BadRequest(new InvalidAuthenticationError(clientId.Value, UserRoles.Client));
    }


    private static async Task<Ok<IEnumerable<UserResponseDto>>> GetClients(
        IClientsService clientsService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string? search = null)
    {
        var clients = await clientsService.GetManyAsync(pagination, search);
        return TypedResults.Ok(clients);
    }


    private static async Task<Results<Ok<UserResponseDto>, BadRequest<ErrorDto>, BadRequest<InvalidAuthenticationError>,
            UnprocessableEntity<ErrorDto>>>
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
            return TypedResults.BadRequest(new ValidationError(validation.ToDictionary()).ToDto());

        var result = await clientsService.UpdateAsync(clientId.Value, dto);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.BadRequest(new InvalidAuthenticationError(clientId.Value, UserRoles.Client)),
            ValidationError error => TypedResults.BadRequest(error.ToDto()),
            _ => TypedResults.UnprocessableEntity(result.Error.ToDto())
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
            : TypedResults.BadRequest(new InvalidAuthenticationError(clientId.Value, UserRoles.Client));
    }
}