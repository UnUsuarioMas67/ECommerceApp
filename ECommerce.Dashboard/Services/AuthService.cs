using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.Results;
using ECommerce.Dashboard.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services;

public class AuthService(
    IOptions<AuthSettings> options,
    IHttpContextAccessor httpContextAccessor,
    HttpClient httpClient)
{
    private const string LoginPath = "api/admins/login";
    private const string RefreshPath = "api/admins/refresh";
    private const string LogoutPath = "api/admins/logout";

    private readonly AuthSettings _authSettings = options.Value;

    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext ??
                                                throw new InvalidOperationException("No active HttpContext was found");

    // public ApiTokens? GetApiTokensFromCookies()
    // {
    //     _httpContext.Request.Cookies.TryGetValue(_authSettings.JwtCookieKey, out var accessToken);
    //     _httpContext.Request.Cookies.TryGetValue(_authSettings.RefreshCookieKey, out var refreshToken);
    //     _httpContext.Request.Cookies.TryGetValue(_authSettings.RefreshCookieKey, out var expiresAt);
    //
    //     if (accessToken == null || refreshToken == null || expiresAt == null)
    //         return null;
    //
    //     var expiresAtDate = DateTime.Parse(expiresAt);
    //
    //     return new ApiTokens(accessToken, refreshToken, expiresAtDate);
    // }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (IsAccessTokenExpiredOrNearExpired())
            await RefreshAsync();
        
        _httpContext.Request.Cookies.TryGetValue(_authSettings.JwtCookieKey, out var accessToken);
     
        return accessToken;
    }

    public async Task<Result> LoginAsync(UserLoginRequest request)
    {
        var response = await httpClient.PostAsJsonAsync(LoginPath, request);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new LoginCredentialsError();

        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                    ?? throw new JsonException("Failed to deserialize response body");

        await SignInAndSaveCookiesAsync(login);

        return Result.Success();
    }
    
    public async Task LogoutAsync()
    {
        var accessToken = await GetAccessTokenAsync();
        if (accessToken == null)
        {
            await SignOutAndDeleteCookiesAsync();
            return;
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        await httpClient.PostAsync(LogoutPath, null);
        await SignOutAndDeleteCookiesAsync();
    }
    
    private async Task RefreshAsync()
    {
        var refreshToken = GetRefreshToken();
        if (refreshToken == null)
        {
            await SignOutAndDeleteCookiesAsync();
            return;
        }

        var response = await httpClient.PostAsJsonAsync(RefreshPath, new { refreshToken });
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return;
        

        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                    ?? throw new JsonException("Failed to deserialize response body");

        await SignInAndSaveCookiesAsync(login);
    }
    
    private string? GetRefreshToken()
    {
        _httpContext.Request.Cookies.TryGetValue(_authSettings.RefreshCookieKey, out var refreshToken);
        return refreshToken;
    }

    private DateTime? GetExpiresAt()
    {
        if (!_httpContext.Request.Cookies.TryGetValue(_authSettings.RefreshCookieKey, out var expiresAt))
            return null;

        return DateTime.Parse(expiresAt);
    }

    private bool IsAccessTokenExpiredOrNearExpired()
    {
        var expiresAt = GetExpiresAt();
        if (!expiresAt.HasValue)
            return true;

        return expiresAt - DateTime.UtcNow <= TimeSpan.FromSeconds(_authSettings.SecondsLeftBeforeRefresh);
    }

    private async Task SignInAndSaveCookiesAsync(UserLoginResponse login)
    {
        SetApiTokenCookies(login.AccessToken, login.RefreshToken, login.ExpiresAt);

        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, login.User.Id.ToString()),
            new Claim(ClaimTypes.Name, login.User.FullName),
            new Claim(ClaimTypes.Email, login.User.Email),
            new Claim(ClaimTypes.MobilePhone, login.User.PhoneNumber),
            new Claim(ClaimTypes.DateOfBirth, login.User.BirthDate.ToString("yyyy-MM-dd")),
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await _httpContext.SignInAsync(principal);
    }

    private async Task SignOutAndDeleteCookiesAsync()
    {
        await _httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        DeleteApiTokenCookies();
    }

    private void SetApiTokenCookies(string accessToken, string refreshToken, DateTime expiresAt)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddDays(5),
            Secure = true,
            SameSite = SameSiteMode.Lax
        };

        _httpContext.Response.Cookies.Append(_authSettings.JwtCookieKey, accessToken, cookieOptions);
        _httpContext.Response.Cookies.Append(_authSettings.RefreshCookieKey, refreshToken, cookieOptions);
        _httpContext.Response.Cookies.Append(_authSettings.JwtExpireCookieKey,
            expiresAt.ToString("YYYY-MM-DDThh:mm:ss"), cookieOptions);
    }

    private void DeleteApiTokenCookies()
    {
        _httpContext.Response.Cookies.Delete(_authSettings.JwtCookieKey);
        _httpContext.Response.Cookies.Delete(_authSettings.RefreshCookieKey);
        _httpContext.Response.Cookies.Delete(_authSettings.JwtExpireCookieKey);
    }
}