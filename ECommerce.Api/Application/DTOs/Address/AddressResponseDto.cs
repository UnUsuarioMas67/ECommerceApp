namespace ECommerce.Api.Application.DTOs.Address;

public class AddressResponseDto
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }

    public string Country { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
}