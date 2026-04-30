using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.DTOs.Product;

public class ProductUpdate
{
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Currency)]
    [Range(0, 10_000_000)]
    public decimal Price { get; set; }
    
    [DataType(DataType.Url)]
    public string? ImageUrl { get; set; }
}