using System.Security.Claims;
using System.Text;
using ECommerce.Api.Entities;
using ECommerce.Api.Settings;
using ECommerce.Api.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Api.Services.Auth;

public class JwtService(IOptions<JwtSettings> settings)
{
    private readonly JwtSettings _settings = settings.Value;

    public JwtToken GenerateAccessToken(Client client)
    {
        var credentials = CreateSigningCredentials();
        var claimsIdentity = CreateClaimsIdentity(client);
        return CreateAccessToken(credentials, claimsIdentity);
    }
    
    public JwtToken GenerateAccessToken(Admin admin)
    {
        var credentials = CreateSigningCredentials();
        var claimsIdentity = CreateClaimsIdentity(admin);
        return CreateAccessToken(credentials, claimsIdentity);
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    private ClaimsIdentity CreateClaimsIdentity(Client client)
    {
        return new ClaimsIdentity([
            new(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new(ClaimTypes.Name, client.FirstName + " " + client.LastName ),
            new(ClaimTypes.Email, client.Email),
            new(ClaimTypes.Role, UserRoles.Client),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        ]);
    }
    
    private ClaimsIdentity CreateClaimsIdentity(Admin admin)
    {
        return new ClaimsIdentity([
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Name, admin.FirstName + " " + admin.LastName ),
            new(ClaimTypes.Email, admin.Email),
            new(ClaimTypes.Role, UserRoles.Admin),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        ]);
    }

    private JwtToken CreateAccessToken(SigningCredentials credentials, ClaimsIdentity claimsIdentity)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
            SigningCredentials = credentials,
            Audience = _settings.Audience,
            Issuer = _settings.Issuer,
        };
        
        var tokenHandler = new JsonWebTokenHandler();
        return new JwtToken(tokenHandler.CreateToken(tokenDescriptor), tokenDescriptor.Expires.Value);
    }
}

public record JwtToken(string Token, DateTime ExpiresAt);