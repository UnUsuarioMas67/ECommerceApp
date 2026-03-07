using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Auth;

public interface IAuthenticationService
{
    Task<Result<AuthenticationDto>> LoginClient(string email, string password);
    Task<Result<AuthenticationDto>> LoginAdmin(string email, string password);
}

public class AuthenticationService(IJwtService jwtService, ECommerceContext context, IClientMapper mapper)
    : IAuthenticationService
{
    // public async Task<Result<AuthenticationDto>> Login<TUser>(string email, string password) where TUser : class, IUser
    // {
    //     var dbSet = dbContext.Set<TUser>();
    //     var user = await dbSet.FirstOrDefaultAsync(u => u.Email == email);
    //     if (user == null)
    //         return Errors.AuthenticationError("Login", "Invalid credentials");
    //
    //     var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    //     if (!passwordValid)
    //         return Errors.AuthenticationError("Login", "Invalid credentials");
    //
    //     var token = jwtService.GenerateAccessToken(user);
    //     return new AuthenticationDto { Token = token, User = user };
    // }

    public async Task<Result<AuthenticationDto>> LoginClient(string email, string password)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        if (client == null)
            return Errors.AuthenticationError("Login", "Invalid credentials");

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, client.PasswordHash);
        if (!passwordValid)
            return Errors.AuthenticationError("Login", "Invalid credentials");

        var token = jwtService.GenerateAccessToken(client);
        return new AuthenticationDto { Token = token, User = mapper.ToDto(client) };
    }

    public Task<Result<AuthenticationDto>> LoginAdmin(string email, string password)
    {
        throw new NotImplementedException();
    }
}