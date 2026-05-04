using ECommerce.Dashboard.DTOs.Category;
using ECommerce.Dashboard.DTOs.Product;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Dashboard.Models;

public class ProductUpdateViewModel
{
    public string ProductSku { get; set; } = string.Empty;
    public ProductUpdate UpdateRequest { get; set; } = new();
    public SelectList CategoriesSelect { get; set; } = new(new List<CategoryResponse>());
}