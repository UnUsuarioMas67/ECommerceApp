using ECommerce.Dashboard.DTOs.User;

namespace ECommerce.Dashboard.DTOs.Auth;

public class UserLoginResponse
{
    public AdminUserResponse User { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}