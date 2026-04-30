using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.DTOs.Category;

public class CategoryRequest
{
    [Required]
    [StringLength(50)]
    public string Slug { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}