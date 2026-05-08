using System.Globalization;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services;

public class CookieHelperService(IOptions<AuthSettings> options)
{
    private readonly AuthSettings _authSettings = options.Value;

    public string? GetRefreshToken(HttpContext context)
    {
        if (!context.Request.Cookies.ContainsKey(_authSettings.RefreshCookieKey))
            return null;
        
        return context.Request.Cookies[_authSettings.RefreshCookieKey];
    }
    
    public void SetApiTokenCookies(UserLoginResponse login, HttpContext context)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddDays(5),
            Secure = true,
            SameSite = SameSiteMode.Lax
        };

        context.Response.Cookies.Append(_authSettings.JwtCookieKey, login.AccessToken, cookieOptions);
        context.Response.Cookies.Append(_authSettings.RefreshCookieKey, login.RefreshToken, cookieOptions);
        context.Response.Cookies.Append(_authSettings.JwtExpireCookieKey,
            login.ExpiresAt.ToString("O", CultureInfo.InvariantCulture), cookieOptions);
    }

    public bool IsAccessTokenExpiredOrNearExpired(HttpContext context)
    {
        if (!context.Request.Cookies.ContainsKey(_authSettings.JwtExpireCookieKey))
            return true;

        var cookie = context.Request.Cookies[_authSettings.JwtExpireCookieKey];

        if (!DateTime.TryParseExact(cookie, "O", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                out var expiresAt))
            return true;

        return expiresAt - DateTime.UtcNow <= TimeSpan.FromSeconds(_authSettings.SecondsLeftBeforeRefresh);
    }
    
    public void DeleteApiTokenCookies(HttpContext context)
    {
        context.Response.Cookies.Delete(_authSettings.JwtCookieKey);
        context.Response.Cookies.Delete(_authSettings.RefreshCookieKey);
        context.Response.Cookies.Delete(_authSettings.JwtExpireCookieKey);
    }
}