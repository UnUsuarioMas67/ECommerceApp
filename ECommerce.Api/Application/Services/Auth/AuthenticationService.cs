using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using ECommerce.Api.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.Auth;

public interface IAuthenticationService
{
    Task<Result<AuthenticationDto>> LoginClient(string email, string password);
    Task<Result<AuthenticationDto>> LoginAdmin(string email, string password);
}

public class AuthenticationService(IJwtService jwtService, ECommerceContext context, ClientMapper mapper)
    : IAuthenticationService
{
    public async Task<Result<AuthenticationDto>> LoginClient(string email, string password)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        if (client == null)
            return new AuthenticationError();

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, client.PasswordHash);
        if (!passwordValid)
            return new AuthenticationError();

        var token = jwtService.GenerateAccessToken(client);
        return new AuthenticationDto { Token = token, User = mapper.MapToDto(client) };
    }

    public Task<Result<AuthenticationDto>> LoginAdmin(string email, string password)
    {
        throw new NotImplementedException();
    }
}