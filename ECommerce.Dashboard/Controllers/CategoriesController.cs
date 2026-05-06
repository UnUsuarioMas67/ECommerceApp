using System.Net;
using ECommerce.Dashboard.DTOs.Category;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Models.Categories;
using ECommerce.Dashboard.Models.Products;
using ECommerce.Dashboard.Results;
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

    public IActionResult Create()
    {
        return View(new CategoryCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var request = new CategoryRequest
        {
            Name = model.Name,
            Slug = model.Slug
        };
        
        var result = await categoryService.CreateCategory(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }

        if (result.Error is ApiTokensError)
            return RedirectToAction("Login", "Account");

        var responseError = result.Error as ApiResponseError;
        var errorMessage = responseError?.ErrorBody?.Message ?? throw new InvalidOperationException();
        TempData["Error"] = errorMessage;

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var categoryResult = await categoryService.GetCategory(id);
        if (!categoryResult.IsSuccess)
            return RedirectToAction("Login", "Account");

        var category = categoryResult.Value;

        var viewModel = new CategoryUpdateViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var request = new CategoryRequest
        {
            Name = model.Name,
            Slug = model.Slug
        };
        
        var result = await categoryService.UpdateCategory(id, request);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Category updated successfully";
            return RedirectToAction(nameof(Index));
        }

        if (result.Error is ApiTokensError)
            return RedirectToAction("Login", "Account");

        var responseError = result.Error as ApiResponseError;
        if (responseError?.StatusCode == HttpStatusCode.NotFound)
            return RedirectToAction(nameof(NotFound));
        
        var errorMessage = responseError?.ErrorBody?.Message ?? throw new InvalidOperationException();
        TempData["Error"] = errorMessage;

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await categoryService.DeleteCategory(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = "Failed to delete product";
        }
        else
        {
            TempData["Success"] = "Product deleted successfully";
        }

        return RedirectToAction(nameof(Index));
    }

    // public async Task<IActionResult> Details(int id)
    // {
    //     var result = await categoryService.GetCategoryByIdOrSlug(id.ToString());
    //     if (result.IsSuccess)
    //         return View(result.Value);
    //
    //     if (result.Error is ApiTokensError)
    //         return RedirectToAction("Login", "Account");
    //
    //     return RedirectToAction(nameof(NotFound));
    // }

    [ActionName("NotFound")]
    public IActionResult ProductNotFound()
    {
        return View();
    }
}