namespace ECommerce.Api.DTOs.Product;

public class ProductCreateDto
{
    public string Category { get; set; }
    
    public string Sku { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int InitialStock { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string __RequestVerificationToken { get; set; }
}