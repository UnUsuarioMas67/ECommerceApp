using System.Security.Claims;
using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Application.DTOs.Shared;
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
        endpoints.MapGet("me", GetAuthClient)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapGet("{id:int}", GetClientById)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapGet("", GetClients)
            .RequireAuthorization(UserRoles.Client);

        endpoints.MapPost("", CreateClient);

        endpoints.MapPut("{id:int}", UpdateClient)
            .RequireAuthorization(UserRoles.Client);

        endpoints.MapDelete("{id:int}", DeleteClient)
            .RequireAuthorization(UserRoles.Client);

        endpoints.MapGet("{id:int}/addresses", GetClientAddresses)
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


    private static async Task<Ok<UserListResponseDto>> GetClients(
        IClientsService clientsService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string? search = null)
    {
        var clients = await clientsService.GetManyAsync(pagination, search);

        var list = new UserListResponseDto
        {
            Users = clients,
            Pagination = pagination,
            SearchTerm = search
        };

        return TypedResults.Ok(list);
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


    private static async Task<Results<Ok<AddressListResponseDto>, NotFound>> GetClientAddresses(
        int id,
        IClientsService clientsService,
        IAddressesService addressesService)
    {
        var client = await clientsService.GetByIdAsync(id);
        if (client == null)
            return TypedResults.NotFound();

        var addresses = await addressesService.GetByClient(id);
        var list = new AddressListResponseDto
        {
            Addresses = addresses,
            Client = client
        };
        
        return TypedResults.Ok(list);
    }
}