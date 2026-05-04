using ECommerce.Dashboard.DTOs.Auth;

namespace ECommerce.Dashboard.Models.Auth;

public class LoginViewModel
{
    public UserLoginRequest LoginRequest { get; set; } = new();
}