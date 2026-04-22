namespace ECommerce.Dashboard.Results;

public record Error(string Type, string Message)
{
    public static Error None() => new Error("none", "No error has occurred");
}

public record LoginCredentialsError() : Error("login_credentials", "Invalid email or password");

public record UnexpectedApiResponseError(int StatusCode, Dictionary<string, object>? ResponseBody = null)
    : Error("api_unexpected_response", "Unexpected API response");

public record ApiServerError(int StatusCode = 500) : Error("api_server_error", "Request failed due to API server error");