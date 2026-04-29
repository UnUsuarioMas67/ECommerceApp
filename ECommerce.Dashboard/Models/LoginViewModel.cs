using ECommerce.Dashboard.Models.Auth;

namespace ECommerce.Dashboard.Models;

public class LoginViewModel
{
    public UserLoginRequest LoginRequest { get; set; } = new();
}