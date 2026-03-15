using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using ECommerce.Api.Shared.Errors;
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

public class AddressesService(ECommerceContext context, IValidator<Address> validator, AddressMapper mapper)
    : IAddressesService
{
    public async Task<AddressResponseDto?> GetByIdAsync(int addressId)
    {
        var address = await context.Addresses
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == addressId);
        return address != null ? mapper.MapToDto(address) : null;
    }

    public async Task<IEnumerable<AddressResponseDto>> GetByClient(int clientId)
        => await context.Addresses
            .Include(a => a.Country)
            .Where(a => a.ClientId == clientId)
            .Select(a => mapper.MapToDto(a))
            .ToListAsync();

    public async Task<IEnumerable<AddressResponseDto>> GetByCountry(string cca2, PaginationQuery pagination)
    {
        return await context.Addresses
            .Include(a => a.Country)
            .Where(a => a.Country!.Cca2 == cca2)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1)).Take(pagination.LimitOrDefault)
            .Select(a => mapper.MapToDto(a))
            .ToListAsync();
    }

    public async Task<Result<AddressResponseDto>> CreateAsync(AddressCreateDto dto)
    {
        var address = await mapper.MapToEntityAsync(dto);

        var verification = await VerifyAddress(address);
        if (!verification.IsSuccess)
            return verification.Error;
        
        await context.Addresses.AddAsync(address);
        await context.SaveChangesAsync();

        return mapper.MapToDto(address);
    }

    public async Task<Result<AddressResponseDto>> UpdateAsync(int addressId, AddressUpdateDto dto)
    {
        var updated = await context.Addresses
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == addressId);

        if (updated == null)
            return new NotFoundError();

        await mapper.ApplyUpdateAsync(updated, dto);
        
        var verification = await VerifyAddress(updated);
        if (!verification.IsSuccess)
        {
            await context.DisposeAsync();
            return verification.Error;
        }

        await context.SaveChangesAsync();

        return mapper.MapToDto(updated);
    }

    public async Task<AddressResponseDto?> DeleteAsync(int addressId)
    {
        var address = await context.Addresses.FindAsync(addressId);
        if (address == null)
            return null;

        context.Addresses.Remove(address);
        await context.SaveChangesAsync();
        return mapper.MapToDto(address);
    }

    public async Task<string?> GetCountryNameAsync(string cca2)
        => await context.Countries
            .Where(c => c.Cca2 == cca2)
            .Select(c => c.Name)
            .FirstOrDefaultAsync();

    private async Task<Result> VerifyAddress(Address address)
    {
        var validation = await validator.ValidateAsync(address);
        if (!validation.IsValid)
            return new ValidationError(validation.ToDictionary());
        if (!await CountryIsValid(address))
            return new InvalidCountryError(address.CountryCca2, address.Id > 0 ? address.Id : null);
        if (!await ClientIsValid(address))
            return new ClientNotExistsError(address.ClientId ?? 0, address.Id > 0 ? address.Id : null);
        
        return Result.Success();
    }
    
    private async Task<bool> CountryIsValid(Address address)
        => await context.Countries.AnyAsync(c => c.Cca2 == address.CountryCca2 || c == address.Country);

    private async Task<bool> ClientIsValid(Address address)
    {
        if (address.Client == null)
            return true;
        
        return await context.Clients.AnyAsync(c => c.Id == address.ClientId || c == address.Client);
    }
}