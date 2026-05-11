using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.DTOs.Auth;

public class UserLoginRequest
{
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
}