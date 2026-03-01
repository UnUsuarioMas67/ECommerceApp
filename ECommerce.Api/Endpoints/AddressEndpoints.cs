using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/addresses/{id:int}", GetById)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapPost("/api/addresses", AddAddress)
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapPut("/api/addresses/{id:int}", UpdateAddress) 
            .RequireAuthorization(UserRoles.Client);
        endpoints.MapDelete("/api/addresses/{id:int}", DeleteAddress)
            .RequireAuthorization(UserRoles.Client);

        endpoints.MapGet("/api/addresses/country/{countryCode}", GetByCountry)
            .RequireAuthorization(UserRoles.Client);

        return endpoints;
    }


    private static async Task<Results<Created<AddressResponseDto>, ValidationProblem>> AddAddress(
        CreateAddressDto dto,
        IAddressesService addressesService,
        IValidator<CreateAddressDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await addressesService.CreateAsync(dto);
        return result.IsSuccess
            ? TypedResults.Created($"/api/addresses/{result.Value!.Id}", result.Value)
            : TypedResults.ValidationProblem(result.Error!.Details);
    }


    private static async Task<Results<Ok<AddressResponseDto>, ValidationProblem, NotFound>> UpdateAddress(
        int id,
        UpdateAddressDto dto,
        IAddressesService addressesService,
        IValidator<UpdateAddressDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await addressesService.UpdateAsync(id, dto);
        if (result is { IsSuccess: false, Error.ErrorType: ErrorType.NotFound })
            return TypedResults.NotFound();

        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.ValidationProblem(result.Error!.Details);
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

    private static async Task<Results<Ok<IEnumerable<AddressResponseDto>>, BadRequest<string>>> GetByCountry(
        IAddressesService addressesService,
        [FromRoute] string countryCode,
        [AsParameters] PaginationQuery pagination
    )
    {
        var result = await addressesService.GetByCountry(countryCode, pagination);
        if (!result.IsSuccess)
            return TypedResults.BadRequest(result.Error!.Details.First().Value[0]);

        return TypedResults.Ok(result.Value);
    }
}