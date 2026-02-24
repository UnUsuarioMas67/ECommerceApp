using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Extensions.Mappings;

public static class AuthenticationMappingExtensions
{
    public static AuthenticationDto GetDto(this Result<JwtToken> result)
        => new AuthenticationDto
        {
            Message = result.Error?.Details.First().Value[0],
            Token = result.Value?.Token,
            User = result.Value?.User.GetDto()
        };
}