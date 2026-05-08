using System.Net;
using System.Text;
using System.Text.Json;
using ECommerce.Dashboard.DTOs.Error;
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
    
    private async Task<HttpResponseMessage> SendAsyncInner(ApiRequestOptions options)
    {
        var httpClient = httpClientFactory.CreateClient("ApiClient");
        
        var request = new HttpRequestMessage(options.Method, options.Path);

        if (options.Body != null)
        {
            var json = JsonSerializer.Serialize(options.Body, _jsonSerializerOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

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
    
    private async Task<Result> ProcessResponse(HttpResponseMessage response)
    {
        var is400Error = (int)response.StatusCode >= 400 && (int)response.StatusCode < 500;
        if (is400Error && response.StatusCode != HttpStatusCode.Unauthorized)
        {
            ApiErrorResponse? errorBody = null;
            if (response.Content.Headers.ContentLength > 0)
                errorBody = await response.Content.ReadFromJsonAsync<ApiErrorResponse>()
                            ?? throw new InvalidOperationException("Could not deserialize the response");

            return new ApiResponseError(response.StatusCode, response.ReasonPhrase, errorBody);
        }
        
        response.EnsureSuccessStatusCode();

        return Result.Success();
    }
    
    private async Task<Result<T>> ProcessResponse<T>(HttpResponseMessage response)
    {
        var isClientError = (int)response.StatusCode >= 400 && (int)response.StatusCode < 500;
        if (isClientError && response.StatusCode != HttpStatusCode.Unauthorized)
        {
            ApiErrorResponse? errorBody = null;
            if (response.Content.Headers.ContentLength > 0)
                errorBody = await response.Content.ReadFromJsonAsync<ApiErrorResponse>()
                            ?? throw new InvalidOperationException("Could not deserialize the response");

            return new ApiResponseError(response.StatusCode, response.ReasonPhrase, errorBody);
        }
        
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new InvalidOperationException("Could not deserialize the response");
        return body;
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