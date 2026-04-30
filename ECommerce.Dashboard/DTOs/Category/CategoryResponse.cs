namespace ECommerce.Dashboard.DTOs.Category;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}