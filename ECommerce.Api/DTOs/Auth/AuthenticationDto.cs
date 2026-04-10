using ECommerce.Api.Application.DTOs.User;

namespace ECommerce.Api.Application.DTOs.Auth;

public class AuthenticationDto
{
    public UserResponseDto? User { get; set; }
    
    public string? Token { get; set; }
}