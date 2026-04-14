using ECommerce.Api.DTOs.Address;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.Mapping;

public class AddressMapper(ECommerceContext context)
{
    public AddressResponseDto MapToDto(Address address)
    {
        if (address.Country == null)
            throw new InvalidOperationException($"{nameof(address.Country)} must be included.");

        return new AddressResponseDto
        {
            Id = address.Id,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            PostalCode = address.PostalCode,
            Region = address.Region,
            ClientId = address.ClientId,
            Country = address.Country!.Name
        };
    }

    public async Task<Address> MapToEntityAsync(AddressCreateDto dto, int clientId)
    {
        var country = await context.Countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);

        return new Address
        {
            ClientId = clientId,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2 ?? "",
            Country = country,
            CountryCca2 = dto.CountryCode,
            Region = dto.Region,
            City = dto.City,
            PostalCode = dto.PostalCode,
        };
    }

    public async Task ApplyUpdateAsync(Address toUpdate, AddressUpdateDto dto)
    {
        if (toUpdate.Country == null)
            throw new InvalidOperationException($"{nameof(toUpdate.Country)} must be included.");

        if (dto.AddressLine1 != null && dto.AddressLine1 != toUpdate.AddressLine1)
            toUpdate.AddressLine1 = dto.AddressLine1;

        if (dto.AddressLine2 != null && dto.AddressLine2 != toUpdate.AddressLine2)
            toUpdate.AddressLine2 = dto.AddressLine2;

        if (dto.City != null && dto.City != toUpdate.City)
            toUpdate.City = dto.City;

        if (dto.PostalCode != null && dto.PostalCode != toUpdate.PostalCode)
            toUpdate.PostalCode = dto.PostalCode;

        if (dto.Region != null && dto.Region != toUpdate.Region)
            toUpdate.Region = dto.Region;

        if (dto.CountryCode != null && dto.CountryCode != toUpdate.Country.Cca2)
        {
            var country = await context.Countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);
            toUpdate.Country = country;
            toUpdate.CountryCca2 = dto.CountryCode;
        }
    }
}