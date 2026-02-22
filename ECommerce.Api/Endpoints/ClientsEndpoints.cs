using System.Security.Claims;
using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Application.Services;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class ClientsEndpoints
{
    public static IEndpointRouteBuilder MapClientsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/clients/me", GetAuthClient)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapGet("/api/clients/{id}", GetClientById)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapGet("/api/clients", GetClients)
            .RequireAuthorization(UserRoles.Client);

        endpoints.MapPost("/api/clients", CreateClient);

        endpoints.MapPut("/api/clients/{id}", UpdateClient)
            .RequireAuthorization(UserRoles.Client);

        endpoints.MapDelete("/api/clients/{id}", DeleteClient)
            .RequireAuthorization(UserRoles.Client);

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
        [FromQuery] string? search = null)
        => TypedResults.Ok(await clientsService.GetClientsAsync(search));


    private static async Task<Results<Created<UserResponseDto>, ValidationProblem>> CreateClient(
        IClientsService clientsService,
        CreateUserDto dto,
        IValidator<CreateUserDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await clientsService.CreateAsync(dto);
        return result.IsSuccess
            ? TypedResults.Created($"/api/clients/{result.Value!.Id}", result.Value)
            : TypedResults.ValidationProblem(result.Error!.Details);
    }


    private static async Task<Results<Ok<UserResponseDto>, ValidationProblem, NotFound>> UpdateClient(
        IClientsService clientsService,
        int id,
        UpdateUserDto dto,
        IValidator<UpdateUserDto> validator)
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