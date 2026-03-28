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
        group.MapGet("{id:int}", GetClientById)
            .WithSummary("Get Client by Id");
        group.MapGet("", GetClients)
            .WithSummary("Get Clients");
        
        group.MapPut("{id:int}", UpdateClient)
            .WithSummary("Update Client");
        group.MapDelete("{id:int}", DeleteClient)
            .WithSummary("Delete Client");

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
    
    private static async Task<Results<Ok<UserResponseDto>, ValidationProblem, NotFound, UnprocessableEntity<Error>>> UpdateClient(
        IClientsService clientsService,
        int id,
        UserUpdateDto dto,
        IValidator<UserUpdateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await clientsService.UpdateAsync(id, dto);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }


    private static async Task<Results<Ok<UserResponseDto>, NotFound>> DeleteClient(
        IClientsService clientsService,
        int id)
    {
        var client = await clientsService.DeleteAsync(id);
        return client != null ? TypedResults.Ok(client) : TypedResults.NotFound();
    }
}