using System.Net.Http.Headers;
using ECommerce.Dashboard.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.DelegateHandlers;

public class ApiAuthorizationHeaderHandler(IOptions<AuthSettings> options, IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    private readonly AuthSettings _authSettings = options.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.Request.Cookies.ContainsKey(_authSettings.JwtCookieKey) == true)
        {
            var accessToken = httpContext.Request.Cookies[_authSettings.JwtCookieKey];
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}