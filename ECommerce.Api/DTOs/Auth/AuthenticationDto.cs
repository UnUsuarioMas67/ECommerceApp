using ECommerce.Api.DTOs.User;

namespace ECommerce.Api.DTOs.Auth;

public class AuthenticationDto
{
    public UserResponseDto? User { get; set; }
    
    public string? Token { get; set; }
}