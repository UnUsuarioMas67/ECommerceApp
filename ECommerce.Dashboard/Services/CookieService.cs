using ECommerce.Dashboard.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services;

public class CookieService(IOptions<AuthSettings> authOptions, IHttpContextAccessor httpContextAccessor)
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

    public void SetApiTokenCookies(ApiTokens apiTokens)
    {
        SetApiTokenCookies(apiTokens.AccessToken, apiTokens.RefreshToken);
    }

    public void SetApiTokenCookies(string accessToken, string refreshToken)
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
}

public record ApiTokens(string AccessToken, string RefreshToken);