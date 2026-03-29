using System.Security.Claims;

namespace ECommerce.Api.Shared;

public static class AuthUser
{
    public static int? GetAuthUserId(HttpContext context)
    {
        var idClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var clientId))
            return null;
        
        return clientId;
    }
}