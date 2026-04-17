using ECommerce.Api.DTOs.User;

namespace ECommerce.Api.DTOs.Auth;

public class AuthenticationDto
{
    public required UserResponseDto? User { get; set; }
    public required string? AccessToken { get; set; }
    public required string? RefreshToken { get; set; }
}