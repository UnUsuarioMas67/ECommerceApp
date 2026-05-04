using System.Net;
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

    public async Task<IActionResult> Create()
    {
        var categoryListResult = await GetCategoriesSelectList();
        if (!categoryListResult.IsSuccess)
            return RedirectToAction("Login", "Account");

        var viewModel = new ProductCreateViewModel
        {
            CreateRequest = new ProductCreate(),
            CategoriesSelect = categoryListResult.Value
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await productService.CreateProduct(model.CreateRequest);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Product created successfully";
            return RedirectToAction(nameof(Index));
        }

        if (result.Error is ApiTokensError)
            return RedirectToAction("Login", "Account");

        var responseError = result.Error as ApiResponseError;
        var errorMessage = responseError?.ErrorBody?.Message ?? throw new InvalidOperationException();
        TempData["Error"] = errorMessage;

        var categoryListResult = await GetCategoriesSelectList();
        if (!categoryListResult.IsSuccess)
            return RedirectToAction("Login", "Account");

        model.CategoriesSelect = categoryListResult.Value;

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var categoryListTask = GetCategoriesSelectList();
        var productTask = productService.GetProductById(id);

        await Task.WhenAll(categoryListTask, productTask);

        var categoryListResult = await categoryListTask;
        var productResult = await productTask;

        if (!categoryListResult.IsSuccess || (!productResult.IsSuccess && productResult.Error is ApiTokensError))
            return RedirectToAction("Login", "Account");
        if (!productResult.IsSuccess)
            return RedirectToAction(nameof(NotFound));

        var product = productResult.Value;

        var model = new ProductUpdateViewModel
        {
            Id = id,
            Sku = product.Sku,
            Name = product.Name,
            Category = product.Category?.Slug ?? string.Empty,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoriesSelect = categoryListResult.Value
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var request = new ProductUpdate
        {
            Name = model.Name,
            Category = model.Category,
            Description = model.Description,
            Price = model.Price,
            ImageUrl = model.ImageUrl
        };

        var result = await productService.UpdateProduct(id, request);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Product updated successfully";
            return RedirectToAction(nameof(Index));
        }

        if (result.Error is ApiTokensError)
            return RedirectToAction("Login", "Account");

        var responseError = result.Error as ApiResponseError;
        if (responseError?.StatusCode == HttpStatusCode.NotFound)
            return RedirectToAction(nameof(NotFound));

        var errorMessage = responseError?.ErrorBody?.Message ?? throw new InvalidOperationException();
        TempData["Error"] = errorMessage;

        var categoryListResult = await GetCategoriesSelectList();
        if (!categoryListResult.IsSuccess)
            return RedirectToAction("Login", "Account");

        model.CategoriesSelect = categoryListResult.Value;

        return View(model);
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

    private async Task<Result<SelectList>> GetCategoriesSelectList()
    {
        var categoriesResult = await categoryService.GetCategories();
        if (!categoriesResult.IsSuccess)
            return categoriesResult.Error;

        var categories = categoriesResult.Value;
        return new SelectList(categories, "Slug", "Name");
    }
}