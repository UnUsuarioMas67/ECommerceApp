using System.Security.Claims;
using System.Text.Json;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.DTOs.User;
using ECommerce.Dashboard.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ECommerce.Dashboard.Middleware;

public class ApiTokenRefreshHandler(RequestDelegate next, CookieHelperService cookieHelperService)
{
    private const string RefreshPath = "api/admins/refresh";
    
    public async Task InvokeAsync(HttpContext context, IHttpClientFactory httpClientFactory)
    {
        if (cookieHelperService.IsAccessTokenExpiredOrNearExpired(context))
        {
            var httpClient = httpClientFactory.CreateClient("ApiClient");
            await RefreshAsync(context, httpClient);
        }

        await next(context);
    }

    private async Task RefreshAsync(HttpContext context, HttpClient httpClient)
    {
        var refreshToken = cookieHelperService.GetRefreshToken(context);
        if (refreshToken == null)
            return;

        var response = await httpClient.PostAsJsonAsync(RefreshPath, new { RefreshToken = refreshToken });
        if (!response.IsSuccessStatusCode)
            return;

        var login = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                    ?? throw new JsonException("Failed to deserialize response body");

        cookieHelperService.SetApiTokenCookies(login, context);
        await RefreshSignInAsync(login.User, context);
    }

    private async Task RefreshSignInAsync(AdminUserResponse user, HttpContext context)
    {
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("yyyy-MM-dd")),
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(principal);
    }
}