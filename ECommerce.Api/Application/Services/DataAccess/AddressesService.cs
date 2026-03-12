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

        var validation = await validator.ValidateAsync(address);
        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());
        
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
            return Errors.NotFound();

        await mapper.ApplyUpdateAsync(updated, dto);

        var validation = await validator.ValidateAsync(updated);
        if (!validation.IsValid)
        {
            await context.DisposeAsync();
            return Errors.ValidationError(validation.ToDictionary());
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
}