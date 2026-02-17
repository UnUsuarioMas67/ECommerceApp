using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Auth;

public interface IAuthenticationService
{
    Task<Result<JwtToken>> Login<TUser>(string email, string password) where TUser : class, IUser;
}

public class AuthenticationService(IJwtService jwtService, ECommerceContext dbContext) : IAuthenticationService
{
    public async Task<Result<JwtToken>> Login<TUser>(string email, string password) where TUser : class, IUser
    {
        var dbSet = dbContext.Set<TUser>();   
        var user = await dbSet.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return Result.Failure<JwtToken>("Invalid credentials");

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!passwordValid)
            return Result.Failure<JwtToken>("Invalid credentials");

        var token = jwtService.GenerateAccessToken(user);
        return Result.Success(new JwtToken { Token = token, User = user });
    }
}

public class JwtToken
{
    public IUser User { get; set; }
    public string Token { get; set; }
}