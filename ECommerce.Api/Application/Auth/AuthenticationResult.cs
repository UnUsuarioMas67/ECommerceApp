using ECommerce.Api.Domain.Entities;

namespace ECommerce.Api.Application.Auth;

public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public IUser? User { get; set; }
    public string? Token { get; set; }
    
    private AuthenticationResult(bool isSuccess, string? message, IUser? client, string? token)
    {
        IsSuccess = isSuccess;
        Message = message;
        User = client;
        Token = token;
    }

    public static AuthenticationResult Success(IUser client, string token)
        => new AuthenticationResult(true, null, client, token);
    
    public static AuthenticationResult Failure(string error)
        => new AuthenticationResult(false, error, null, null);
}