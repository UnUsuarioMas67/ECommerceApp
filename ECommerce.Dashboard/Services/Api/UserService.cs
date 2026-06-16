using ECommerce.Dashboard.DTOs.User;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class UserService(ApiRequestService apiRequestService)
{
    private const string AdminPath = "api/admins";
    
    public async Task<AdminUserResponse> GetCurrent()
    {
        var options = new ApiRequestOptions
        {
            Path = AdminPath + "/me",
            Method = HttpMethod.Get,
        };
        
        return await apiRequestService.SendAlwaysSucceedAsync<AdminUserResponse>(options);
    }

    public async Task<Result<AdminUserResponse>> UpdateUser(AdminUserUpdate request)
    {
        var options = new ApiRequestOptionsJson
        {
            Path = AdminPath,
            Method = HttpMethod.Put,
            Body = request
        };
        
        return await apiRequestService.SendAsync<AdminUserResponse>(options);
    }
}