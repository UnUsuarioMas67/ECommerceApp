using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECommerce.Api.Services.Auth;

public class RefreshTokenService(ECommerceContext context, IOptions<JwtSettings> options)
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<ClientRefreshToken> GenerateRefreshTokenAsync(Client client)
    {
        var token = new ClientRefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            Client = client,
            ClientId = client.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
        };

        await context.ClientRefreshTokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token;
    }

    public async Task<AdminRefreshToken> GenerateRefreshTokenAsync(Admin admin)
    {
        var token = new AdminRefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            Admin = admin,
            AdminId = admin.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
        };

        await context.AdminRefreshTokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token;
    }

    public async Task<ClientRefreshToken?> GetClientRefreshTokenAsync(string token)
    {
        var entry = await context.ClientRefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
        return entry;
    }

    public async Task<AdminRefreshToken?> GetAdminRefreshTokenAsync(string token)
    {
        var entry = await context.AdminRefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
        return entry;
    }

    public async Task DeleteByClientIdAsync(int clientId)
    {
        var refreshTokens = await context.ClientRefreshTokens
            .Where(rt => rt.ClientId == clientId)
            .ToListAsync();
        
        context.ClientRefreshTokens.RemoveRange(refreshTokens);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByAdminIdAsync(int adminId)
    {
        var refreshTokens = await context.AdminRefreshTokens
            .Where(rt => rt.AdminId == adminId)
            .ToListAsync();
        
        context.AdminRefreshTokens.RemoveRange(refreshTokens);
        await context.SaveChangesAsync();
    }

    public async Task DeleteTokenAsync(ClientRefreshToken token)
    {
        context.ClientRefreshTokens.Remove(token);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteTokenAsync(AdminRefreshToken token)
    {
        context.AdminRefreshTokens.Remove(token);
        await context.SaveChangesAsync();
    }
}