namespace ECommerce.Api.Application.DTOs.Address;

// ReSharper disable once ClassNeverInstantiated.Global
public class AddressUpdateDto
{
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    
    public string? CountryCode { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
}