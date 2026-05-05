using ECommerce.Dashboard.DTOs.Category;

namespace ECommerce.Dashboard.Models.Categories;

public class CategoryListViewModel
{
    public List<CategoryResponse> Categories { get; set; } = new();
    public string? SearchTerm { get; set; }
}