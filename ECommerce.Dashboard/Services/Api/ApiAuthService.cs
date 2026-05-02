using System.Net;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.Models;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class ApiAuthService(
    ApiRequestService apiRequestService,
    ILogger<ApiAuthService> logger)
{
    private const string LoginPath = "api/admins/login";
    private const string GetUserPath = "api/admins/me";
    private const string LogoutPath = "api/admins/logout";

    public async Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest request)
    {
        logger.LogInformation("Requesting login: {email}", request.Email);

        var result = await apiRequestService.SendAsync<UserLoginResponse>(new ApiRequestOptions
        {
            Method = HttpMethod.Post,
            Path = LoginPath,
            Body = request,
            SendToken = false,
            ExpectedFailCodes = [HttpStatusCode.Unauthorized]
        });

        if (!result.IsSuccess)
            return new LoginCredentialsError();

        return result.Value;
    }

    public async Task<Result<AdminUser>> GetAuthenticatedUserAsync()
    {
        return await apiRequestService.SendAsync<AdminUser>(new ApiRequestOptions
        {
            Method = HttpMethod.Get,
            Path = GetUserPath,
        });
    }

    public async Task<Result> LogoutAsync()
    {
        return await apiRequestService.SendAsync(new ApiRequestOptions
        {
            Method = HttpMethod.Post,
            Path = LogoutPath,
        });
    }
}