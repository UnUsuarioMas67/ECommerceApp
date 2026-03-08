using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Application.Services.Mapping;

public interface IClientMapper : IEntityDtoMapper<Client, UserResponseDto, UserCreateDto, UserUpdateDto>;

public class ClientMapper : IClientMapper
{
    public UserResponseDto ToDto(Client client)
    {
        return new UserResponseDto
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            BirthDate = client.BirthDate,
            CreatedAt = client.CreatedAt,
        };
    }

    public Client ToEntity(UserCreateDto dto)
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

    public Client GetUpdatedEntity(Client client, UserUpdateDto dto)
    {
        var updated = PropertyCopier.GetCopy(client);

        updated.FirstName = dto.FirstName ?? client.FirstName;
        updated.LastName = dto.LastName ?? client.LastName;
        updated.PhoneNumber = dto.PhoneNumber ?? client.PhoneNumber;
        updated.BirthDate = !string.IsNullOrWhiteSpace(dto.BirthDate)
            ? DateOnly.Parse(dto.BirthDate)
            : client.BirthDate;
        updated.PasswordHash = !string.IsNullOrWhiteSpace(dto.Password)
            ? PasswordHasher.HashPassword(dto.Password)
            : client.PasswordHash;

        return updated;
    }
}