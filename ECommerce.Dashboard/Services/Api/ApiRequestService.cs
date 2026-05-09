using System.Net;
using System.Text;
using System.Text.Json;
using ECommerce.Dashboard.DTOs.Error;
using ECommerce.Dashboard.Results;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Services.Api;

public class ApiRequestService(
    IOptions<JsonOptions> jsonOptions,
    IHttpClientFactory httpClientFactory)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions.Value.SerializerOptions;

    public async Task<Result> SendAsync(ApiRequestOptions options)
    {
        var response = await SendAsyncInner(options);
        return await ProcessResponse(response);
    }
    
    public async Task<Result<T>> SendAsync<T>(ApiRequestOptions options)
    {
        var response = await SendAsyncInner(options);
        return await ProcessResponse<T>(response);
    }
    
    public async Task<T> SendAlwaysSucceedAsync<T>(ApiRequestOptions options)
    {
        var response = await SendAsyncInner(options);
        
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new InvalidOperationException("Could not deserialize the response");
        return body;
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

        return await httpClient.SendAsync(request);
    }
    
    private async Task<Result> ProcessResponse(HttpResponseMessage response)
    {
        var error = await ExtractErrorFromResponse(response);
        if (error != null)
            return error;
        
        return Result.Success();
    }
    
    private async Task<Result<T>> ProcessResponse<T>(HttpResponseMessage response)
    {
        var error = await ExtractErrorFromResponse(response);
        if (error != null)
            return error;
        
        var body = await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new InvalidOperationException("Could not deserialize the response");
        return body;
    }

    private async Task<Error?> ExtractErrorFromResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new ApiNotFoundResponseError();
        
        var is400Error = (int)response.StatusCode >= 400 && (int)response.StatusCode < 500;
        if (is400Error && response.StatusCode != HttpStatusCode.Unauthorized)
        {
            var errorBody = await response.Content.ReadFromJsonAsync<ApiErrorResponse>()
                            ?? throw new InvalidOperationException("Could not deserialize the response");

            return new ApiFailureResponseError(response.StatusCode, response.ReasonPhrase, errorBody);
        }
        
        response.EnsureSuccessStatusCode();

        return null;
    }
}

public record ApiRequestOptions
{
    public required HttpMethod Method { get; init; }
    public required string Path { get; init; }
    public object? Body { get; init; }
}