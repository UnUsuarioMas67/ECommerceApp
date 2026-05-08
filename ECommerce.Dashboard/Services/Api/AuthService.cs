using System.Net;
using System.Text.Json;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class AuthService(IHttpClientFactory httpClientFactory)
{
    private const string LoginPath = "api/admins/login";
    private const string LogoutPath = "api/admins/logout";

    public async Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest request)
    {
        var httpClient = httpClientFactory.CreateClient("ApiClient");
        
        var response = await httpClient.PostAsJsonAsync(LoginPath, request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new LoginCredentialsError();

        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                    ?? throw new JsonException("Failed to deserialize response body");

        return login;
    }
    
    public async Task LogoutAsync()
    {
        var httpClient = httpClientFactory.CreateClient("ApiClient");
        await httpClient.PostAsync(LogoutPath, null);
    }
}