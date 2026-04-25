using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerce.Dashboard.Exceptions;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Results;
using ECommerce.Dashboard.Settings;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services.Api;

public class ApiRequestService(
    IHttpClientFactory clientFactory,
    IHttpContextAccessor httpContextAccessor,
    ILogger<ApiRequestService> logger,
    IOptions<AuthSettings> authOptions,
    IOptions<JsonOptions> jsonOptions)
{
    private const string RefreshPath = "api/admins/refresh";

    private readonly HttpClient _httpClient = clientFactory.CreateClient("ApiClient");
    private readonly AuthSettings _authSettings = authOptions.Value;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions.Value.SerializerOptions;

    public async Task<Result<HttpResponseMessage>> SendAsync(ApiRequestOptions options)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new InvalidOperationException("HttpContext is null");
        
        var hasAccessToken = httpContext.Request.Cookies.TryGetValue(_authSettings.JwtCookieKey, out var accessToken);
        var hasRefreshToken = httpContext.Request.Cookies.TryGetValue(_authSettings.RefreshCookieKey, out var refreshToken);

        if ((!hasAccessToken || !hasRefreshToken) && options.SendToken)
            return new MissingTokenCookiesError();

        return await SendAsyncInner(options, accessToken, refreshToken);
    }

    private async Task<Result<HttpResponseMessage>> SendAsyncInner(
        ApiRequestOptions options,
        string? accessToken = null,
        string? refreshToken = null,
        bool refreshIfExpired = true
    )
    {
        var request = new HttpRequestMessage(options.Method, options.Path);

        if (options.Body != null)
        {
            var json = JsonSerializer.Serialize(options.Body, _jsonSerializerOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        if (options.SendToken)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode || options.ExpectedFailCodes.Contains(response.StatusCode))
            return response;

        if (response.StatusCode == HttpStatusCode.Unauthorized && options.SendToken && refreshIfExpired)
        {
            var refreshResult = await RefreshAsync(refreshToken ?? "");
            if (!refreshResult.IsSuccess)
                return refreshResult.Error;

            var login = refreshResult.Value;
            return await SendAsyncInner(options, login.AccessToken, login.RefreshToken, false);
        }

        throw new UnexpectedApiResponseException(response.StatusCode);
    }

    private async Task<Result<UserLoginResponse>> RefreshAsync(string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync(RefreshPath, new { RefreshToken = refreshToken });
        if (response.IsSuccessStatusCode)
        {
            var login = await response.Content.ReadFromJsonAsync<UserLoginResponse>()
                        ?? throw new InvalidOperationException("Could not deserialize the response");
            logger.LogDebug("Refresh request successful. User: {admin}", login.User.Email);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(5),
            };

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is null");

            httpContext.Response.Cookies.Append(_authSettings.JwtCookieKey, login.AccessToken, cookieOptions);
            httpContext.Response.Cookies.Append(_authSettings.RefreshCookieKey, login.RefreshToken, cookieOptions);

            return login;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogInformation("Refresh request failed. Invalid or expired refresh token");
            return new RefreshTokenError();
        }

        throw new UnexpectedApiResponseException(response.StatusCode);
    }

    private void ThrowExceptions(HttpResponseMessage response)
    {
        // if ((int)response.StatusCode >= 500)
        //     throw new ApiServerException(response.StatusCode);
        //
        // var errorBody = response.Content.Headers.ContentLength > 0
        //     ? await response.Content.ReadFromJsonAsync<Dictionary<string, object>>()
        //     : new Dictionary<string, object>();
        throw new UnexpectedApiResponseException(response.StatusCode);
    }
}

public record ApiRequestOptions
{
    public required HttpMethod Method { get; init; }
    public required string Path { get; init; }
    public object? Body { get; init; }
    public HttpStatusCode[] ExpectedFailCodes { get; init; } = [];
    public bool SendToken { get; init; } = true;
}