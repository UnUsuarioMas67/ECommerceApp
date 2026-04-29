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

        var result = await apiRequestService.SendAsync(new ApiRequestOptions
        {
            Method = HttpMethod.Post,
            Path = LoginPath,
            Body = request,
            SendToken = false,
            ExpectedFailCodes = [HttpStatusCode.Unauthorized]
        });

        if (!result.IsSuccess)
            return result.Error;

        var response = result.Value;

        if (!response.IsSuccessStatusCode)
        {
            logger.LogInformation("Login request failed. Invalid login credentials");
            return new LoginCredentialsError();
        }

        var login = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                    ?? throw new InvalidOperationException("Could not deserialize the response");
        logger.LogInformation("Login request successful. User: {admin}", login.User.Email);
        
        return login;
    }

    public async Task<Result<AdminUser>> GetAuthenticatedUserAsync()
    {
        var result = await apiRequestService.SendAsync(new ApiRequestOptions
        {
            Method = HttpMethod.Get,
            Path = GetUserPath,
        });

        if (!result.IsSuccess)
            return result.Error;

        var response = result.Value;

        var user = await response.Content.ReadFromJsonAsync<AdminUser>()
                   ?? throw new InvalidOperationException("Could not deserialize the response");

        return user;
    }

    public async Task<Result> LogoutAsync()
    {
        var result = await apiRequestService.SendAsync(new ApiRequestOptions
        {
            Method = HttpMethod.Post,
            Path = LogoutPath,
        });

        return result;
    }
}