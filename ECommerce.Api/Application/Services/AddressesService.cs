using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;

namespace ECommerce.Api.Application.Services;

public interface IAddressesService
{
    Task<AddressResponseDto?> GetByIdAsync(int addressId);
    Task<IEnumerable<AddressResponseDto>> GetAddressesAsync(string? search = null);
    Task<IEnumerable<AddressResponseDto>> GetByClientId(int clientId);
    Task<Result<AddressResponseDto>> CreateAsync(CreateAddressDto dto);
    Task<Result<AddressResponseDto>> UpdateAsync(int addressId, UpdateAddressDto dto);
    Task<AddressResponseDto?> DeleteAsync(int addressId);
}

public class AddressesService(ECommerceContext db, IValidator<Address> validator) : IAddressesService
{
    public Task<AddressResponseDto?> GetByIdAsync(int addressId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AddressResponseDto>> GetAddressesAsync(string? search = null)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AddressResponseDto>> GetByClientId(int clientId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AddressResponseDto>> CreateAsync(CreateAddressDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AddressResponseDto>> UpdateAsync(int addressId, UpdateAddressDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<AddressResponseDto?> DeleteAsync(int addressId)
    {
        throw new NotImplementedException();
    }
}