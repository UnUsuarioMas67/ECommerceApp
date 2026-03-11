using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.Mapping;

public interface IAddressMapper : IEntityDtoAsyncMapper<Address, AddressResponseDto, AddressCreateDto, AddressUpdateDto>;

public class AddressMapper : IAddressMapper
{
    public AddressResponseDto ToDto(Address address)
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

    public async Task<Address> ToEntityAsync(AddressCreateDto dto, ECommerceContext context)
    {
        var country = await context.Countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);

        return new Address
        {
            ClientId = dto.ClientId,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2 ?? "",
            Country = country,
            CountryCca2 = dto.CountryCode,
            Region = dto.Region,
            City = dto.City,
            PostalCode = dto.PostalCode,
        };
    }

    public async Task ApplyUpdateToEntityAsync(Address toUpdate, AddressUpdateDto dto, ECommerceContext context)
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