namespace ECommerce.Api.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public string CountryCca2 { get; set; }
    public int? ClientId { get; set; }
    
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Region { get; set; }

    public Country? Country { get; set; }
    public Client? Client { get; set; }
}