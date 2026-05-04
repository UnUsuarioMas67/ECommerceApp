using System.ComponentModel.DataAnnotations;
using ECommerce.Dashboard.DTOs.Category;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Dashboard.Models;

public class ProductUpdateViewModel
{
    public int Id { get; set; }
    
    public string Sku { get; set; } = string.Empty;
    
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
    
    public SelectList CategoriesSelect { get; set; } = new(new List<CategoryResponse>());
}