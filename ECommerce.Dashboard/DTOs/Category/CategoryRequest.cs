using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.DTOs.Category;

public class CategoryRequest
{
    public required string Slug { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
}