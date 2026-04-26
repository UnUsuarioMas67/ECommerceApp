using ECommerce.Api.DTOs.Auth;
using ECommerce.Api.EF;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Mapping;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.Auth;

public class AuthenticationService(
    JwtService jwtService,
    RefreshTokenService refreshTokenService,
    ECommerceContext context,
    ClientMapper clientMapper,
    AdminMapper adminMapper)
{
    public async Task<Result<AuthenticationDto>> LoginClient(string email, string password)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Email == email);
        if (client == null)
            return new InvalidLoginError();

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, client.PasswordHash);
        if (!passwordValid)
            return new InvalidLoginError();

        var accessToken = jwtService.GenerateAccessToken(client);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(client);

        return new AuthenticationDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            User = clientMapper.MapToDto(client)
        };
    }

    public async Task<Result<AuthenticationDto>> LoginAdmin(string email, string password)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Email == email);
        if (admin == null)
            return new InvalidLoginError();

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
        if (!passwordValid)
            return new InvalidLoginError();

        var accessToken = jwtService.GenerateAccessToken(admin);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(admin);

        return new AuthenticationDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            User = adminMapper.MapToDto(admin)
        };
    }

    public async Task<Result<AuthenticationDto>> RefreshClientToken(string refreshToken)
    {
        var refreshTokenEntry = await refreshTokenService.GetClientRefreshTokenAsync(refreshToken);
        if (refreshTokenEntry == null || refreshTokenEntry.ExpiresAt < DateTime.UtcNow)
            return new RefreshTokenError();

        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == refreshTokenEntry.ClientId);
        if (client == null)
            return new RefreshTokenError();
        
        await refreshTokenService.DeleteByClientIdAsync(client.Id);
        
        var accessToken = jwtService.GenerateAccessToken(client);
        var newRefreshToken = await refreshTokenService.GenerateRefreshTokenAsync(client);
        
        return new AuthenticationDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            User = clientMapper.MapToDto(client)
        };
    }

    public async Task<Result<AuthenticationDto>> RefreshAdminToken(string refreshToken)
    {
        var refreshTokenEntry = await refreshTokenService.GetAdminRefreshTokenAsync(refreshToken);
        if (refreshTokenEntry == null || refreshTokenEntry.ExpiresAt < DateTime.UtcNow)
            return new RefreshTokenError();

        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Id == refreshTokenEntry.AdminId);
        if (admin == null)
            return new RefreshTokenError();
        
        await refreshTokenService.DeleteByAdminIdAsync(admin.Id);

        var accessToken = jwtService.GenerateAccessToken(admin);
        var newRefreshToken = await refreshTokenService.GenerateRefreshTokenAsync(admin);
        
        return new AuthenticationDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            User = adminMapper.MapToDto(admin)
        };
    }

    public async Task LogoutClient(int clientId)
    {
        await refreshTokenService.DeleteByClientIdAsync(clientId);
    }

    public async Task LogoutAdmin(int adminId)
    {
        await refreshTokenService.DeleteByAdminIdAsync(adminId);
    }
}