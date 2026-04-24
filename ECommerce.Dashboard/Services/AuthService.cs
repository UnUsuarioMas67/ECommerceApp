using System.Net;
using System.Net.Http.Headers;
using ECommerce.Dashboard.Exceptions;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services;

public class AuthService(IHttpClientFactory clientFactory, ILogger<AuthService> logger)
{
    private const string LoginPath = "api/admins/login";
    private const string LogoutPath = "api/admins/logout";
    private const string RefreshPath = "api/admins/refresh";
    private const string GetUserPath = "api/admins/me";

    private readonly HttpClient _httpClient = clientFactory.CreateClient("ApiClient");

    public async Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest request)
    {
        logger.LogInformation("Requesting login: {email}", request.Email);
        
        var response = await _httpClient.PostAsJsonAsync(LoginPath, request);
        if (response.IsSuccessStatusCode)
        {
            var userLogin = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                            ?? throw new InvalidOperationException("Could not deserialize the response");
            logger.LogInformation("Login request successful. User: {admin}", userLogin.User.Email);
            return userLogin;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogInformation("Login request failed. Invalid login credentials");
            return new LoginCredentialsError();
        }

        if ((int)response.StatusCode >= 500)
            throw new ApiServerException(response.StatusCode);

        var errorBody = response.Content.Headers.ContentLength > 0
            ? await response.Content.ReadFromJsonAsync<Dictionary<string, object>>()
            : new Dictionary<string, object>();
        throw new UnexpectedApiResponseException(response.StatusCode, errorBody);
    }
    
    public async Task<Result<UserLoginResponse>> RefreshAsync(string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync(RefreshPath, new {RefreshToken = refreshToken});
        if (response.IsSuccessStatusCode)
        {
            var userLogin = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                            ?? throw new InvalidOperationException("Could not deserialize the response");
            logger.LogDebug("Refresh request successful. User: {admin}", userLogin.User.Email);
            return userLogin;
        }
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogInformation("Refresh request failed. Invalid or expired refresh token");
            return new RefreshTokenError();
        }
        
        if ((int)response.StatusCode >= 500)
            throw new ApiServerException(response.StatusCode);

        var errorBody = response.Content.Headers.ContentLength > 0
            ? await response.Content.ReadFromJsonAsync<Dictionary<string, object>>()
            : new Dictionary<string, object>();
        throw new UnexpectedApiResponseException(response.StatusCode, errorBody);
    }

    public async Task<Result<AdminUser>> GetAuthenticatedUserAsync(string jwt)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        
        var response = await _httpClient.GetAsync(GetUserPath);
        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<AdminUser>()
                            ?? throw new InvalidOperationException("Could not deserialize the response");
            return user;
        }
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new LoginCredentialsError();
        }
        
        if ((int)response.StatusCode >= 500)
            throw new ApiServerException(response.StatusCode);

        
        var errorBody = response.Content.Headers.ContentLength > 0
            ? await response.Content.ReadFromJsonAsync<Dictionary<string, object>>()
            : new Dictionary<string, object>();
        throw new UnexpectedApiResponseException(response.StatusCode, errorBody);
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