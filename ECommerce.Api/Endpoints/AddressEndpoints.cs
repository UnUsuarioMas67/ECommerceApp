using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.DataAccess;
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
        group.MapGet("country/{countryCode}", GetByCountry);

        return endpoints;
    }


    private static async Task<Results<Created<AddressResponseDto>, ValidationProblem>> AddAddress(
        HttpContext httpContext,
        AddressCreateDto dto,
        IAddressesService addressesService,
        IValidator<AddressCreateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await addressesService.CreateAsync(dto);
        var path = httpContext.Request.Path;
        return result.IsSuccess
            ? TypedResults.Created($"{path}/{result.Value!.Id}", result.Value)
            : TypedResults.ValidationProblem(result.Error!.Details);
    }


    private static async Task<Results<Ok<AddressResponseDto>, ValidationProblem, NotFound>> UpdateAddress(
        int id,
        AddressUpdateDto dto,
        IAddressesService addressesService,
        IValidator<AddressUpdateDto> validator)
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

    private static async Task<Ok<AddressListResponseDto>> GetByCountry(
        IAddressesService addressesService,
        [FromRoute] string countryCode,
        [AsParameters] PaginationQuery pagination
    )
    {
        var addresses = await addressesService.GetByCountry(countryCode, pagination);
        var countryName = await addressesService.GetCountryNameAsync(countryCode);

        var list = new AddressListResponseDto
        {
            Addresses = addresses,
            Pagination = pagination,
            Country = countryName
        };

        return TypedResults.Ok(list);
    }
}