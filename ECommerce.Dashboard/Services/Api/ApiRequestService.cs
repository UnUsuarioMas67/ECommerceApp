using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.Exceptions;
using ECommerce.Dashboard.Results;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services.Api;

public class ApiRequestService(
    IHttpClientFactory clientFactory,
    ILogger<ApiRequestService> logger,
    SiteAuthService siteAuthService,
    IOptions<JsonOptions> jsonOptions)
{
    private const string RefreshPath = "api/admins/refresh";

    private readonly HttpClient _httpClient = clientFactory.CreateClient("ApiClient");
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions.Value.SerializerOptions;

    public async Task<Result<HttpResponseMessage>> SendAsync(ApiRequestOptions options)
    {
        var tokens = siteAuthService.GetApiTokensFromCookies();
        
        if (tokens == null && options.SendToken)
            return new MissingTokenCookiesError();

        return await SendAsyncInner(options, tokens?.AccessToken, tokens?.RefreshToken);
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

            await siteAuthService.SignInAsync(login.User, login.AccessToken, login.RefreshToken);

            return login;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogInformation("Refresh request failed. Invalid or expired refresh token");
            await siteAuthService.SignOutAsync();
            return new RefreshTokenError();
        }

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