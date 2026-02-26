using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Extensions.Mappings;

public static class ClientMappingExtensions
{
    public static UserResponseDto GetDto(this IUser user)
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


    public static Client GetEntity(this CreateUserDto dto)
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