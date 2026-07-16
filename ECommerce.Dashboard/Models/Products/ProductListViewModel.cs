using ECommerce.Dashboard.DTOs.Category;
using ECommerce.Dashboard.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Dashboard.Models.Products;

public class ProductListViewModel
{
    public List<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    public SelectList CategorySelect { get; set; } = new SelectList(new List<CategoryResponse>(), "Slug", "Name");
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
    public required int Page { get; set; }
    public required int TotalPages { get; set; }
}