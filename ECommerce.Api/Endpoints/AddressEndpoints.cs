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
            .RequireAuthorization(UserRoles.Client);

        group.MapGet("{id:int}", GetById);
        group.MapPost("", AddAddress);
        group.MapPut("{id:int}", UpdateAddress);
        group.MapDelete("{id:int}", DeleteAddress);
        group.MapGet("country/{countryCode}", GetAddressesByCountry);
        group.MapGet("clients/{id:int}", GetAddressesByClient);

        return endpoints;
    }


    private static async Task<Results<Created<AddressResponseDto>, ValidationProblem, UnprocessableEntity<Error>>>
        AddAddress(
            HttpContext httpContext,
            AddressCreateDto dto,
            IAddressesService addressesService,
            IValidator<AddressCreateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await addressesService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);

        return TypedResults.UnprocessableEntity(result.Error);
    }


    private static async Task<Results<Ok<AddressResponseDto>, ValidationProblem, NotFound, UnprocessableEntity<Error>>>
        UpdateAddress(
            int id,
            AddressUpdateDto dto,
            IAddressesService addressesService,
            IValidator<AddressUpdateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await addressesService.UpdateAsync(id, dto);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.NotFound(),
            ValidationError error => TypedResults.ValidationProblem(error.Details),
            _ => TypedResults.UnprocessableEntity(result.Error)
        };
    }


    private static async Task<Results<Ok<AddressResponseDto>, NotFound>> DeleteAddress(
        int id,
        IAddressesService addressesService)
    {
        var deleted = await addressesService.DeleteAsync(id);
        if (deleted == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(deleted);
    }


    private static async Task<Results<Ok<AddressResponseDto>, NotFound>> GetById(
        int id,
        IAddressesService addressesService)
    {
        var address = await addressesService.GetByIdAsync(id);
        if (address == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(address);
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

    private static async Task<Ok<IEnumerable<AddressResponseDto>>> GetAddressesByClient(
        int id,
        IAddressesService addressesService)
    {
        var addresses = await addressesService.GetByClient(id);
        return TypedResults.Ok(addresses);
    }
}