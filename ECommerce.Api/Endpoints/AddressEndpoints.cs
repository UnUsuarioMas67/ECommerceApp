using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Errors;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/addresses")
            .WithTags("Address")
            .RequireAuthorization(UserRoles.Client);

        group.MapPost("", AddAddress);
        group.MapPut("{id:int}", UpdateAddress);
        group.MapDelete("{id:int}", DeleteAddress);
        group.MapGet("me", GetAuthClientAddresses);
        group.MapGet("{id:int}", GetAddressById);

        // group.MapGet("country/{countryCode}", GetAddressesByCountry);

        return endpoints;
    }


    private static async Task<Results<Created<AddressResponseDto>, ValidationProblem,
            BadRequest<InvalidAuthenticationError>, UnprocessableEntity<Error>>>
        AddAddress(
            HttpContext httpContext,
            AddressCreateDto dto,
            IAddressesService addressesService,
            IValidator<AddressCreateDto> validator)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await addressesService.CreateAsync(dto, clientId.Value);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);

        return TypedResults.UnprocessableEntity(result.Error);
    }


    private static async Task<Results<Ok<AddressResponseDto>, ValidationProblem, NotFound,
            BadRequest<InvalidAuthenticationError>, UnprocessableEntity<Error>>>
        UpdateAddress(
            HttpContext httpContext,
            int id,
            AddressUpdateDto dto,
            IAddressesService addressesService,
            IValidator<AddressUpdateDto> validator)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await addressesService.UpdateAsync(id, dto, clientId);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }


    private static async Task<Results<Ok<AddressResponseDto>, NotFound, BadRequest<InvalidAuthenticationError>>>
        DeleteAddress(
            HttpContext httpContext,
            int id,
            IAddressesService addressesService)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var deleted = await addressesService.DeleteAsync(id, clientId);
        if (deleted == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(deleted);
    }


    private static async Task<Results<Ok<AddressResponseDto>, NotFound>> GetAddressById(
        int id,
        IAddressesService addressesService)
    {
        var address = await addressesService.GetByIdAsync(id);
        if (address == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(address);
    }

    private static async Task<Results<Ok<IEnumerable<AddressResponseDto>>, BadRequest<InvalidAuthenticationError>>>
        GetAuthClientAddresses(
            HttpContext httpContext,
            IAddressesService addressesService)
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (clientId == null)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var addresses = await addressesService.GetByClient(clientId.Value);
        return TypedResults.Ok(addresses);
    }

    private static async Task<Ok<IEnumerable<AddressResponseDto>>> GetAddressesByCountry(
        IAddressesService addressesService,
        [FromRoute] string countryCode,
        [AsParameters] PaginationQuery pagination
    )
    {
        var addresses = await addressesService.GetByCountry(countryCode, pagination);
        return TypedResults.Ok(addresses);
    }
}