using ECommerce.Api.DTOs.Auth;
using ECommerce.Api.EF;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Mapping;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.Auth;

public interface IAuthenticationService
{
    Task<Result<AuthenticationDto>> LoginClient(string email, string password);
    Task<Result<AuthenticationDto>> LoginAdmin(string email, string password);
}

public class AuthenticationService(JwtService jwtService, ECommerceContext context, ClientMapper clientMapper, AdminMapper adminMapper)
    : IAuthenticationService
{
    public async Task<Result<AuthenticationDto>> LoginClient(string email, string password)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        if (client == null)
            return new InvalidLoginError();

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, client.PasswordHash);
        if (!passwordValid)
            return new InvalidLoginError();

        var token = jwtService.GenerateAccessToken(client);
        return new AuthenticationDto { Token = token, User = clientMapper.MapToDto(client) };
    }

    public async Task<Result<AuthenticationDto>> LoginAdmin(string email, string password)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        if (admin == null)
            return new InvalidLoginError();

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
        if (!passwordValid)
            return new InvalidLoginError();

        var token = jwtService.GenerateAccessToken(admin);
        return new AuthenticationDto { Token = token, User = adminMapper.MapToDto(admin) };
    }
}