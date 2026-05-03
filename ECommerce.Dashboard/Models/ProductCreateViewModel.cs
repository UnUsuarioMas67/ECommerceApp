using ECommerce.Dashboard.DTOs.Category;
using ECommerce.Dashboard.DTOs.Product;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Dashboard.Models;

public class ProductCreateViewModel
{
    public ProductCreate CreateRequest { get; set; } = new();
    public SelectList CategoriesSelect { get; set; } = new SelectList(new List<CategoryResponse>());
}