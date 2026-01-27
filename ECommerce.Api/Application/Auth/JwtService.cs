using System.Security.Claims;
using System.Text;
using ECommerce.Api.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Api.Application.Auth;

public interface IJwtService
{
    string GenerateAccessToken(Client client);
}

public class JwtService(IOptions<JwtSettings> settings) : IJwtService
{
    private readonly JwtSettings _settings = settings.Value;
    
    public string GenerateAccessToken(Client client)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsIdentity = new ClaimsIdentity([
            new(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new(ClaimTypes.Name, client.FirstName + " " + client.LastName ),
            new(ClaimTypes.Email, client.Email),
            new(ClaimTypes.Role, UserRoles.Client),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        ]);

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
        return tokenHandler.CreateToken(tokenDescriptor);
    }
}