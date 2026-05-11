namespace ECommerce.Dashboard.DTOs.Client;

public class ClientResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = string.Empty;
}