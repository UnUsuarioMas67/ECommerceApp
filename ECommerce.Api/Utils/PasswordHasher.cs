namespace ECommerce.Api.Utils;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        return passwordHash;
    }
}