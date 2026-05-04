using System.Security.Claims;
using ECommerce.Dashboard.Models;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services;

public class SiteAuthService(IOptions<AuthSettings> authOptions, IHttpContextAccessor httpContextAccessor)
{
    private readonly AuthSettings _authSettings = authOptions.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext ??
                                                throw new InvalidOperationException("No active HttpContext was found");

    public ApiTokens? GetApiTokensFromCookies()
    {
        _httpContext.Request.Cookies.TryGetValue(_authSettings.JwtCookieKey, out var accessToken);
        _httpContext.Request.Cookies.TryGetValue(_authSettings.RefreshCookieKey, out var refreshToken);
        
        if (accessToken == null || refreshToken == null)
            return null;
        
        return new ApiTokens(accessToken, refreshToken);
    }

    public async Task SignInAsync(AdminUser user, string accessToken, string refreshToken)
    {
        SetApiTokenCookies(accessToken, refreshToken);
        
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("yyyy-MM-dd")),
        ], CookieAuthenticationDefaults.AuthenticationScheme);
        
        var principal = new ClaimsPrincipal(identity);

        await _httpContext.SignInAsync(principal);
    }

    public async Task SignOutAsync()
    {
        await _httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        DeleteApiTokenCookies();
    }
    
    private void SetApiTokenCookies(string accessToken, string refreshToken)
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
    }

    private void DeleteApiTokenCookies()
    {
        _httpContext.Response.Cookies.Delete(_authSettings.JwtCookieKey);
        _httpContext.Response.Cookies.Delete(_authSettings.RefreshCookieKey);
    }
}

public record ApiTokens(string AccessToken, string RefreshToken);