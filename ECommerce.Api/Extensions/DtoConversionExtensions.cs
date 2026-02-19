using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Extensions;

public static class DtoConversionExtensions
{
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

    public static AuthenticationDto ToDto(this Result<JwtToken> result)
        => new AuthenticationDto
        {
            Message = result.Error?.Details.First().Value[0],
            Token = result.Value?.Token,
            User = result.Value?.User.ToDto()
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

    public static void UpdateFromDto(this Client client, UpdateUserDto dto)
    {
        client.FirstName = !string.IsNullOrEmpty(dto.FirstName) ? dto.FirstName : client.FirstName;

        client.LastName = !string.IsNullOrEmpty(dto.LastName) ? dto.LastName : client.LastName;

        client.PhoneNumber = !string.IsNullOrEmpty(dto.PhoneNumber) ? dto.PhoneNumber : client.PhoneNumber;

        client.BirthDate = !string.IsNullOrWhiteSpace(dto.BirthDate)
            ? DateOnly.Parse(dto.BirthDate)
            : client.BirthDate;

        client.PasswordHash = !string.IsNullOrWhiteSpace(dto.Password)
            ? PasswordHasher.HashPassword(dto.Password)
            : client.PasswordHash;
    }
}