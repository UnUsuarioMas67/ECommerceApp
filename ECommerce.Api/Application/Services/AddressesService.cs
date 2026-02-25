using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Extensions.Mappings;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services;

public interface IAddressesService
{
    Task<AddressResponseDto?> GetByIdAsync(int addressId);
    Task<IEnumerable<AddressResponseDto>> GetByClient(int clientId);
    Task<IEnumerable<AddressResponseDto>> GetByCountry(string cca2);
    Task<Result<AddressResponseDto>> CreateAsync(CreateAddressDto dto);
    Task<Result<AddressResponseDto>> UpdateAsync(int addressId, UpdateAddressDto dto);
    Task<AddressResponseDto?> DeleteAsync(int addressId);
}

public class AddressesService(ECommerceContext context, IValidator<Address> validator) : IAddressesService
{
    public async Task<AddressResponseDto?> GetByIdAsync(int addressId)
    {
        var address = await context.Addresses
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == addressId);
        return address?.GetDto();
    }

    public async Task<IEnumerable<AddressResponseDto>> GetByClient(int clientId)
        => await context.Addresses
            .Include(a => a.Country)
            .Where(a => a.ClientId == clientId)
            .Select(a => a.GetDto())
            .ToListAsync();

    public async Task<IEnumerable<AddressResponseDto>> GetByCountry(string cca2)
        => await context.Addresses
            .Include(a => a.Country)
            .Where(a => a.Country!.Cca2 == cca2)
            .Select(a => a.GetDto())
            .ToListAsync();

    public async Task<Result<AddressResponseDto>> CreateAsync(CreateAddressDto dto)
    {
        var address = await dto.GetEntityAsync(context);

        var validationResult = await Validate(address);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        await context.Addresses.AddAsync(address);
        await context.SaveChangesAsync();

        return address.GetDto();
    }

    public async Task<Result<AddressResponseDto>> UpdateAsync(int addressId, UpdateAddressDto dto)
    {
        var address = await context.Addresses
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == addressId);
        
        if (address == null)
            return Errors.NotFound();

        var updated = await address.GetUpdatedAsync(dto, context);

        var validationResult = await Validate(updated);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        ApplyAddressUpdate(address, updated);
        await context.SaveChangesAsync();

        return address.GetDto();
    }

    private void ApplyAddressUpdate(Address address, Address updated)
    {
        address.Id = updated.Id;
        address.ClientId = updated.ClientId;
        address.AddressLine1 = updated.AddressLine1;
        address.AddressLine2 = updated.AddressLine2;
        address.CountryCca2 = updated.CountryCca2;
        address.Country = updated.Country;
        address.Region = updated.Region;
        address.City = updated.City;
        address.PostalCode = updated.PostalCode;
    }

    public async Task<AddressResponseDto?> DeleteAsync(int addressId)
    {
        var address = await context.Addresses.FindAsync(addressId);
        if (address == null)
            return null;

        context.Addresses.Remove(address);
        await context.SaveChangesAsync();
        return address.GetDto();
    }

    private async Task<Result> Validate(Address address)
    {
        var validation = await validator.ValidateAsync(address);
        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());

        var clientValid = await ClientIsValid(address);
        var countryValid = await CountryIsValid(address);

        if (clientValid && countryValid)
            return Result.Success();
        
        var errorDetails = new Dictionary<string, string[]>();
        
        if (!clientValid)
            errorDetails.Add(nameof(address.Client), ["Invalid client"]);
        if (!countryValid)
            errorDetails.Add(nameof(address.Country), ["Invalid country"]);
        
        return Errors.ValidationError(errorDetails);
    }

    private async Task<bool> ClientIsValid(Address address)
    {
        if (address.Client == null && address.ClientId == 0)
            return true;

        return await context.Clients.AnyAsync(c => c.Id == address.ClientId || c == address.Client);
    }

    private async Task<bool> CountryIsValid(Address address)
        => await context.Countries.AnyAsync(
            c => c.Cca2 == address.CountryCca2 || c == address.Country);
}