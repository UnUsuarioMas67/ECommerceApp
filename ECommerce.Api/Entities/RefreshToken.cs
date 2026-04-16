namespace ECommerce.Api.Entities;

public class ClientRefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    public int ClientId { get; set; }
    public Client Client { get; set; }
}

public class AdminRefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    
    public int AdminId { get; set; }
    public Admin Admin { get; set; }
}

