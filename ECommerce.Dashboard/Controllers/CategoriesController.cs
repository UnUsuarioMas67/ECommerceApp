using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Models.Categories;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

public class CategoriesController(CategoryService categoryService) : Controller
{
    // GET
    public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
    {
        var categoryResult = await categoryService.GetCategories(searchTerm, new PaginationQuery(20, page));
        if (!categoryResult.IsSuccess)
            return RedirectToAction("Login", "Account");

        var categories = categoryResult.Value;

        var viewModel = new CategoryListViewModel
        {
            Categories = categories.ToList(),
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }
}