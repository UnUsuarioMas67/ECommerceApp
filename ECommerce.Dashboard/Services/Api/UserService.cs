using ECommerce.Dashboard.DTOs;
using ECommerce.Dashboard.DTOs.User;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class UserService(ApiRequestService apiRequestService)
{
    private const string AdminPath = "api/admins";
    
    public async Task<AdminUser> GetCurrent()
    {
        var options = new ApiRequestOptions
        {
            Path = AdminPath + "/me",
            Method = HttpMethod.Get,
        };
        
        return await apiRequestService.SendAlwaysSucceedAsync<AdminUser>(options);
    }

    public async Task<Result<AdminUser>> UpdateUser(AdminUserUpdate request)
    {
        var options = new ApiRequestOptions
        {
            Path = AdminPath,
            Method = HttpMethod.Put,
            Body = request
        };
        
        return await apiRequestService.SendAsync<AdminUser>(options);
    }
}