using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.DTOs.Product;

public class ProductCreate
{
    public required string Category { get; set; } = string.Empty;
    public required string Sku { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required decimal Price { get; set; }
    public required int InitialStock { get; set; }
    public required IFormFile? ImageFile { get; set; }
}