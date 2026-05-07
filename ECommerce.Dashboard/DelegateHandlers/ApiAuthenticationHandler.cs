using System.Net;
using System.Net.Http.Headers;
using ECommerce.Dashboard.Services;

namespace ECommerce.Dashboard.DelegateHandlers;

public class ApiAuthenticationHandler(AuthService authService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await authService.GetAccessTokenAsync();
        if (accessToken == null)
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await base.SendAsync(request, cancellationToken);
        
        return response;
    }
}