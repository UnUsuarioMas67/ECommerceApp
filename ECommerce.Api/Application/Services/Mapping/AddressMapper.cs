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

    public async Task<Address> GetUpdatedEntityAsync(Address address, AddressUpdateDto dto, ECommerceContext context)
    {
        if (address.Country == null)
            throw new InvalidOperationException($"{nameof(address.Country)} must be included.");

        var updated = PropertyCopier.GetCopy(address);

        updated.AddressLine1 = dto.AddressLine1 ?? address.AddressLine1;
        updated.AddressLine2 = dto.AddressLine2 ?? address.AddressLine2;
        updated.City = dto.City ?? address.City;
        updated.PostalCode = dto.PostalCode ?? address.PostalCode;
        updated.Region = dto.Region ?? address.Region;

        if (dto.CountryCode != null)
        {
            var country = await context.Countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);
            updated.Country = country;
            updated.CountryCca2 = dto.CountryCode;
        }

        return updated;
    }
}