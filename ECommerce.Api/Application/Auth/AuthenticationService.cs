using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Auth;

public interface IAuthenticationService
{
    Task<AuthenticationResult> Login<TUser>(string email, string password) where TUser : class, IUser;
}

public class AuthenticationService(IJwtService jwtService, ECommerceContext dbContext) : IAuthenticationService
{
    public async Task<AuthenticationResult> Login<TUser>(string email, string password) where TUser : class, IUser
    {
        var dbSet = dbContext.Set<TUser>();   
        var user = await dbSet.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return AuthenticationResult.Failure("Invalid credentials");

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!passwordValid)
            return AuthenticationResult.Failure("Invalid credentials");

        var token = jwtService.GenerateAccessToken(user);
        return AuthenticationResult.Success(user, token);
    }
}