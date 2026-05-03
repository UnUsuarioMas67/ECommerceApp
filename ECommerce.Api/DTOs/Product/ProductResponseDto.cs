using ECommerce.Api.DTOs.Category;

namespace ECommerce.Api.DTOs.Product;

public class ProductResponseDto
{
    public int Id { get; set; }
    
    public CategoryResponseDto? Category { get; set; }
    
    public string Sku { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public string? ImageUrl { get; set; }
}