using System.Net;
using ECommerce.Dashboard.DTOs.Error;

namespace ECommerce.Dashboard.Results;

public record Error(string ErrorType, string Message)
{
    public static Error None() => new Error("none", "No error has occurred");
}

public record LoginCredentialsError() : Error("login_credentials", "Invalid email or password");

public record ApiTokensError : Error
{
    private ApiTokensError(string message)
        : base("api_tokens", message)
    {
    }

    public static ApiTokensError MissingCookies => new("Missing API token cookies");
    public static ApiTokensError RefreshToken => new("Invalid or expired refresh token");
}

public record ApiResponseError(HttpStatusCode StatusCode, string? Message, ApiErrorResponse? ErrorBody = null)
    : Error("api_response", Message ?? "API returned failure response");

public record UnexpectedApiResponseError(int StatusCode, Dictionary<string, object>? ResponseBody = null)
    : Error("api_unexpected_response", "Unexpected API response");

public record ApiServerError(int StatusCode = 500) : Error("api_server_error", "Request failed due to API server error");