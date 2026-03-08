using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface IAddressesService
{
    Task<AddressResponseDto?> GetByIdAsync(int addressId);
    Task<IEnumerable<AddressResponseDto>> GetByClient(int clientId);
    Task<IEnumerable<AddressResponseDto>> GetByCountry(string cca2, PaginationQuery pagination);
    Task<Result<AddressResponseDto>> CreateAsync(AddressCreateDto dto);
    Task<Result<AddressResponseDto>> UpdateAsync(int addressId, AddressUpdateDto dto);
    Task<AddressResponseDto?> DeleteAsync(int addressId);
    Task<string?> GetCountryNameAsync(string cca2);
}

public class AddressesService(ECommerceContext context, IValidator<Address> validator, IAddressMapper mapper) 
    : IAddressesService
{
    public async Task<AddressResponseDto?> GetByIdAsync(int addressId)
    {
        var address = await context.Addresses
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == addressId);
        return address != null ? mapper.ToDto(address) : null;
    }

    public async Task<IEnumerable<AddressResponseDto>> GetByClient(int clientId)
        => await context.Addresses
            .Include(a => a.Country)
            .Where(a => a.ClientId == clientId)
            .Select(a => mapper.ToDto(a))
            .ToListAsync();

    public async Task<IEnumerable<AddressResponseDto>> GetByCountry(string cca2, PaginationQuery pagination)
    {
        return await context.Addresses
            .Include(a => a.Country)
            .Where(a => a.Country!.Cca2 == cca2)
            .Skip(pagination.Skip ?? PaginationDefaults.Skip).Take(pagination.Limit ?? PaginationDefaults.Limit)
            .Select(a => mapper.ToDto(a))
            .ToListAsync();
    }

    public async Task<Result<AddressResponseDto>> CreateAsync(AddressCreateDto dto)
    {
        var address = await mapper.ToEntityAsync(dto, context);

        var validationResult = await Validate(address);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        await context.Addresses.AddAsync(address);
        await context.SaveChangesAsync();

        return mapper.ToDto(address);
    }

    public async Task<Result<AddressResponseDto>> UpdateAsync(int addressId, AddressUpdateDto dto)
    {
        var address = await context.Addresses
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == addressId);
        
        if (address == null)
            return Errors.NotFound();

        var updated = await mapper.UpdateEntityAsync(address, dto, context);

        var validationResult = await Validate(updated);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        PropertyCopier.Mirror(updated, address);
        await context.SaveChangesAsync();

        return mapper.ToDto(address);
    }

    public async Task<AddressResponseDto?> DeleteAsync(int addressId)
    {
        var address = await context.Addresses.FindAsync(addressId);
        if (address == null)
            return null;

        context.Addresses.Remove(address);
        await context.SaveChangesAsync();
        return mapper.ToDto(address);
    }

    public async Task<string?> GetCountryNameAsync(string cca2)
        => await context.Countries
            .Where(c => c.Cca2 == cca2)
            .Select(c => c.Name)
            .FirstOrDefaultAsync();
    

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