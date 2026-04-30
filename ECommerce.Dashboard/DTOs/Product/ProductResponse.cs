namespace ECommerce.Dashboard.DTOs.Product;

public class ProductResponse
{
    public int Id { get; set; }
    
    public string? Category { get; set; }
    
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public string? ImageUrl { get; set; }
}