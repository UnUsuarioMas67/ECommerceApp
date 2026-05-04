using ECommerce.Dashboard.DTOs.Category;
using ECommerce.Dashboard.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Dashboard.Models.Products;

public class ProductListViewModel
{
    public List<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    public SelectList CategorySelect { get; set; } = new SelectList(new List<CategoryResponse>(), "Slug", "Name");
    [FromQuery(Name = "category")] public string? Category { get; set; }
    [FromQuery(Name = "searchTerm")] public string? SearchTerm { get; set; }
}