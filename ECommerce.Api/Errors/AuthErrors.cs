namespace ECommerce.Api.Errors;

public record InvalidLoginError() : Error("Invalid login credentials");

public record InvalidAuthenticationError : Error
{
    public int? UserId { get; set; }
    public string? UserRole { get; set; }

    public InvalidAuthenticationError(int userId, string userRole)
        : base("User does not have permission to access this resource")
    {
        UserId = userId;
        UserRole = userRole;
    }

    public InvalidAuthenticationError()
        : base("Missing user identifier")
    {
    }
}