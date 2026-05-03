using ECommerce.Dashboard.DTOs.Product;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Models;
using ECommerce.Dashboard.Results;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Dashboard.Controllers;

[Authorize]
public class ProductsController(ProductService productService, CategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string? searchTerm = null, string? category = null)
    {
        var productTask = productService.GetProducts(searchTerm, new PaginationQuery(20, page), category);
        var categoryTask = categoryService.GetCategories();

        await Task.WhenAll(categoryTask, productTask);
        
        var productResult = await productTask;
        var categoryResult = await categoryTask;
        
        if (!categoryResult.IsSuccess || !productResult.IsSuccess)
            return RedirectToAction("Login", "Account");

        var products = productResult.Value;
        var categories = categoryResult.Value;

        var viewModel = new ProductListViewModel
        {
            Products = products.ToList(),
            CategorySelect = new SelectList(categories, "Slug", "Name"),
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await productService.GetProductById(id);
        if (result.IsSuccess)
            return View(result.Value);

        if (result.Error is ApiTokensError)
            return RedirectToAction("Login", "Account");
        
        return RedirectToAction(nameof(NotFound));
    }

    public IActionResult Create()
    {
        return View(new ProductCreate());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreate model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await productService.CreateProduct(model);
        if (!result.IsSuccess)
        {
            TempData["Error"] = "Failed to create product";
            return View(model);
        }

        TempData["Success"] = "Product created successfully";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await productService.GetProductById(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = "Product not found";
            return RedirectToAction(nameof(Index));
        }

        var product = result.Value;
        var model = new ProductUpdate
        {
            Category = product.Category?.Slug ?? string.Empty,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl
        };

        ViewData["ProductId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductUpdate model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await productService.UpdateProduct(id, model);
        if (!result.IsSuccess)
        {
            TempData["Error"] = "Failed to update product";
            return View(model);
        }

        TempData["Success"] = "Product updated successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await productService.DeleteProduct(id);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restock(int id, int amount)
    {
        if (amount <= 0)
        {
            TempData["Error"] = "Amount must be greater than zero";
            return RedirectToAction(nameof(Details), new { id });
        }

        var result = await productService.RestockProduct(id, amount);
        if (!result.IsSuccess)
        {
            TempData["Error"] = "Failed to restock product";
        }
        else
        {
            TempData["Success"] = "Product restocked successfully";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [ActionName("NotFound")]
    public IActionResult ProductNotFound()
    {
        return View();
    }
}