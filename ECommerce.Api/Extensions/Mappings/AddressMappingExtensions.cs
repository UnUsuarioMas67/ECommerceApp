using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions.Mappings;

public static class AddressMappingExtensions
{
    public static AddressResponseDto GetDto(this Address address)
        => new()
        {
            Id = address.Id,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            PostalCode = address.PostalCode,
            Region = address.Region,
            ClientId = address.ClientId,
            Country = address.Country.Name
        };

    public static async Task<Address> GetEntityAsync(this CreateAddressDto dto, ECommerceContext context)
    {
        var country = await context.Countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);
        
        return new Address
        {
            ClientId = dto.ClientId,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2 ?? "",
            CountryId = country?.Id ?? 0,
            Region = dto.Region,
            City = dto.City,
            PostalCode = dto.PostalCode,
        };
    }

    public static async Task<Address> GetUpdatedAsync(this Address address, UpdateAddressDto dto, ECommerceContext context)
    {
        var updated =  new Address
        {
            Id = address.Id,
            ClientId = address.ClientId,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            CountryId = address.CountryId,
            City = address.City,
            PostalCode = address.PostalCode,
            Region = address.Region
        };
        
        updated.AddressLine1 = dto.AddressLine1 ?? address.AddressLine1;
        updated.AddressLine2 = dto.AddressLine2 ?? address.AddressLine2;
        updated.City = dto.City ?? address.City;
        updated.PostalCode = dto.PostalCode ?? address.PostalCode;
        updated.Region = dto.Region ?? address.Region;
        
        var country = await context.Countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);
        updated.CountryId = country?.Id ?? 0;

        return updated;
    }
}