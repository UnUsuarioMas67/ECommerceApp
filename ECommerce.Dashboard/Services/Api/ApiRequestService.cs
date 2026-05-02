using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.DTOs.Error;
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

    public async Task<Result> SendAsync(ApiRequestOptions options)
    {
        var tokens = siteAuthService.GetApiTokensFromCookies();
        
        if (tokens == null && options.SendToken)
            return ApiTokensError.MissingCookies;

        var response = await SendAsyncInner(options, tokens?.AccessToken, tokens?.RefreshToken);
        if (response == null)
            return ApiTokensError.RefreshToken;

        return await ProcessResponse(response);
    }
    
    private async Task<Result> ProcessResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        ApiErrorResponse? errorBody = null;
        if (response.Content.Headers.ContentLength > 0)
            errorBody = await response.Content.ReadFromJsonAsync<ApiErrorResponse>()
                        ?? throw new InvalidOperationException("Could not deserialize the response");

        return new ApiResponseError(response.StatusCode, response.ReasonPhrase, errorBody);
    }
    
    public async Task<Result<T>> SendAsync<T>(ApiRequestOptions options)
    {
        var tokens = siteAuthService.GetApiTokensFromCookies();
        
        if (tokens == null && options.SendToken)
            return ApiTokensError.MissingCookies;

        var response = await SendAsyncInner(options, tokens?.AccessToken, tokens?.RefreshToken);
        if (response == null)
            return ApiTokensError.RefreshToken;

        return await ProcessResponse<T>(response);
    }

    private async Task<Result<T>> ProcessResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadFromJsonAsync<T>()
                          ?? throw new InvalidOperationException("Could not deserialize the response");
            return body;
        }

        ApiErrorResponse? errorBody = null;
        if (response.Content.Headers.ContentLength > 0)
            errorBody = await response.Content.ReadFromJsonAsync<ApiErrorResponse>()
                       ?? throw new InvalidOperationException("Could not deserialize the response");

        return new ApiResponseError(response.StatusCode, response.ReasonPhrase, errorBody);
    }

    private async Task<HttpResponseMessage?> SendAsyncInner(
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
            var login = await RefreshAsync(refreshToken ?? "");
            if (login == null)
                return null;

            return await SendAsyncInner(options, login.AccessToken, login.RefreshToken, false);
        }

        throw new UnexpectedApiResponseException(response.StatusCode);
    }

    private async Task<UserLoginResponse?> RefreshAsync(string refreshToken)
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
            return null;
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