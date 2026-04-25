using System.Net;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Results;
using ECommerce.Dashboard.Services.Api;
using ECommerce.Dashboard.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services;

public class AuthService(
    ApiRequestService apiRequestService,
    IOptions<AuthSettings> authSettings,
    IHttpContextAccessor httpContextAccessor,
    ILogger<AuthService> logger)
{
    private const string LoginPath = "api/admins/login";
    private const string GetUserPath = "api/admins/me";
    private const string LogoutPath = "api/admins/logout";

    private readonly AuthSettings _authSettings = authSettings.Value;

    public async Task<Result<AdminUser>> LoginAsync(UserLoginRequest request)
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

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddDays(5),
        };

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            httpContext.Response.Cookies.Append(_authSettings.JwtCookieKey, login.AccessToken, cookieOptions);
            httpContext.Response.Cookies.Append(_authSettings.RefreshCookieKey, login.RefreshToken, cookieOptions);
        }

        return login.User;
    }

    public async Task<Result<AdminUser>> GetAuthenticatedUserAsync()
    {
        var result = await apiRequestService.SendAsync(new ApiRequestOptions
        {
            Method = HttpMethod.Get,
            Path = GetUserPath,
            SendToken = true
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
        throw new NotImplementedException();

        // var response = await _httpClient.PostAsync(LogoutPath, null);
        // if (response.IsSuccessStatusCode)
        // {
        //     return Result.Success();
        // }
        //
        // if (response.StatusCode == HttpStatusCode.Unauthorized)
        // {
        //     return new RefreshTokenError();
        // }
        //
        // if ((int)response.StatusCode >= 500)
        // {
        //     var error = new ApiServerError((int)response.StatusCode);
        //     logger.LogWarning("{error}", error.Message);
        //     return error;
        // }
        //
        // var errorBody = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        // var responseError = new UnexpectedApiResponseError((int)response.StatusCode, errorBody);
        //
        // var json = JsonSerializer.Serialize(responseError, new JsonSerializerOptions { WriteIndented = true });
        // logger.LogWarning("Unexpected response: {error}", json);
        //
        // return responseError;
    }
}