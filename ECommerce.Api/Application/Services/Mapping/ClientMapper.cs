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

    public void ApplyUpdateToEntity(Client toUpdate, UserUpdateDto dto)
    {
        if (dto.FirstName != null && dto.FirstName != toUpdate.FirstName)
            toUpdate.FirstName = dto.FirstName;

        if (dto.LastName != null && dto.LastName != toUpdate.LastName)
            toUpdate.LastName = dto.LastName;

        if (dto.PhoneNumber != null && dto.PhoneNumber != toUpdate.PhoneNumber)
            toUpdate.PhoneNumber = dto.PhoneNumber;

        if (DateOnly.TryParseExact(dto.BirthDate, "yyyy-MM-dd", out var birthDate) && birthDate != toUpdate.BirthDate)
            toUpdate.BirthDate = birthDate;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (newPasswordHash != toUpdate.PasswordHash)
                toUpdate.PasswordHash = newPasswordHash;
        }
    }
}