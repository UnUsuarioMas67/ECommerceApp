using System.Net;
using ECommerce.Dashboard.DTOs.Error;

namespace ECommerce.Dashboard.Results;

public record Error(string ErrorType, string Message)
{
    public static Error None() => new Error("none", "No error has occurred");
}

public record LoginCredentialsError() : Error("login_credentials", "Invalid email or password");

public record ApiFailureResponseError(HttpStatusCode StatusCode, string? Message, ApiErrorResponse? ErrorBody = null)
    : Error("api_error_400", Message ?? "API returned failure response");
    
public record ApiNotFoundResponseError() : Error("api_error_404", "API returned NOT FOUND");