using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions;

public static class DtoConversionExtensions
{
    public static AuthenticationDto ToDto(this Result<JwtToken> result)
        => new AuthenticationDto
        {
            Message = result.Error?.Details.First().Value[0],
            Token = result.Value?.Token,
            User = result.Value?.User.ToDto()
        };

    #region Users

    public static UserResponseDto ToDto(this IUser user)
        => new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            BirthDate = user.BirthDate,
            CreatedAt = user.CreatedAt,
        };


    public static Client ToClientEntity(this CreateUserDto dto)
        => new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = PasswordHasher.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            BirthDate = DateOnly.Parse(dto.BirthDate),
            CreatedAt = DateTime.UtcNow,
        };

    public static Client GetUpdated(this Client client, UpdateUserDto dto)
        => new()
        {
            Id = client.Id,
            FirstName = !string.IsNullOrEmpty(dto.FirstName) ? dto.FirstName : client.FirstName,
            LastName = !string.IsNullOrEmpty(dto.LastName) ? dto.LastName : client.LastName,
            PhoneNumber = !string.IsNullOrEmpty(dto.PhoneNumber) ? dto.PhoneNumber : client.PhoneNumber,

            BirthDate = !string.IsNullOrWhiteSpace(dto.BirthDate)
                ? DateOnly.Parse(dto.BirthDate)
                : client.BirthDate,

            PasswordHash = !string.IsNullOrWhiteSpace(dto.Password)
                ? PasswordHasher.HashPassword(dto.Password)
                : client.PasswordHash,

            CreatedAt = client.CreatedAt,
            Email = client.Email
        };

    #endregion

    #region Addresses

    public static AddressResponseDto? ToDto(this Address address)
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

    public static async Task<Address> GetEntityAsync(this CreateAddressDto dto, DbSet<Country> countries)
    {
        var country = await countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);
        
        return new Address
        {
            ClientId = dto.ClientId,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            CountryId = country?.Id ?? 0,
            Region = dto.Region,
            City = dto.City,
            PostalCode = dto.PostalCode,
        };
    }

    public static async Task<Address> GetUpdatedAsync(this Address address, UpdateAddressDto dto, DbSet<Country> countries)
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
        
        updated.AddressLine1 = string.IsNullOrWhiteSpace(dto.AddressLine1) ? address.AddressLine1 : dto.AddressLine1;
        updated.City = string.IsNullOrWhiteSpace(dto.City) ? address.City : dto.City;
        updated.PostalCode = string.IsNullOrWhiteSpace(dto.PostalCode) ? address.PostalCode : dto.PostalCode;
        updated.Region = string.IsNullOrWhiteSpace(dto.Region) ? address.Region : dto.Region;

        if (dto.AddressLine2 != null && dto.AddressLine2.Trim() == "")
            updated.AddressLine2 = null;
        else if (dto.AddressLine2 != null)
            updated.AddressLine2 = dto.AddressLine2;
        
        var country = await countries.FirstOrDefaultAsync(c => c.Cca2 == dto.CountryCode);
        updated.CountryId = country?.Id ?? 0;

        return updated;
    }

    #endregion
}