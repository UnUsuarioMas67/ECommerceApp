namespace ECommerce.Dashboard.DTOs.Order;

public class Address
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}