using ECommerce.Dashboard.Models;

namespace ECommerce.Dashboard.DTOs.Auth;

public class UserLoginResponse
{
    public AdminUser User { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}