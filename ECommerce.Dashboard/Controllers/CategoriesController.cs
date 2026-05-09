using System.Net;
using ECommerce.Dashboard.DTOs.Category;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Filters;
using ECommerce.Dashboard.Models.Categories;
using ECommerce.Dashboard.Results;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

[Authorize]
[TypeFilter<HandleApi401Exception>]
public class CategoriesController(CategoryService categoryService) : Controller
{
    // GET
    public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
    {
        var categories = await categoryService.GetCategories(searchTerm, new PaginationQuery(20, page));
        
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

        var responseError = result.Error as ApiFailureResponseError;
        var errorMessage = responseError?.ErrorBody.Message;
        TempData["Error"] = errorMessage;

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var categoryResult = await categoryService.GetCategory(id);

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

        if (result.Error is ApiNotFoundResponseError)
            return RedirectToAction(nameof(NotFound));
        
        var responseError = result.Error as ApiFailureResponseError;
        var errorMessage = responseError?.ErrorBody.Message;
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

    [ActionName("NotFound")]
    public IActionResult ProductNotFound()
    {
        return View();
    }
}