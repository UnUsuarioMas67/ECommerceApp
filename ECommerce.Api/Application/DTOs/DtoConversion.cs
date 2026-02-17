using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Entities;

namespace ECommerce.Api.Application.DTOs;

public static class DtoConversion
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
            Message = result.ErrorMessage,
            Token = result.Value?.Token,
            User = result.Value?.User.ToDto()
        };
}