using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.DTOs.Product;

public class ProductUpdate
{
    public required string Category { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required decimal Price { get; set; }
    public required string? ImageUrl { get; set; }
}