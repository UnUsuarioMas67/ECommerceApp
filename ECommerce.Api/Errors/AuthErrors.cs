using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record InvalidLoginError() : Error("Invalid login credentials", "invalid_credentials");

public record RefreshTokenError() : Error("Invalid or expired refresh token", "refresh_token_invalid");

public record InvalidAuthenticationError : Error
{
    public int? UserId { get; set; }
    public string? UserRole { get; set; }

    public InvalidAuthenticationError(int userId, string userRole)
        : base("User does not have permission to access this resource", "forbidden")
    {
        UserId = userId;
        UserRole = userRole;
    }

    public InvalidAuthenticationError()
        : base("Missing user identifier", "user_not_found")
    {
    }

    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["userId"] = UserId,
            ["userRole"] = UserRole
        }
    };
}