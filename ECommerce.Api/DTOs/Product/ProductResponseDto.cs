namespace ECommerce.Api.Application.DTOs.Product;

public class ProductResponseDto
{
    public int Id { get; set; }
    
    public string? Category { get; set; }
    
    public string Sku { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public string? ImageUrl { get; set; }
}