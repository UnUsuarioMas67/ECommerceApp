using System.Net;
using ECommerce.Dashboard.DTOs.Product;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Filters;
using ECommerce.Dashboard.Models;
using ECommerce.Dashboard.Models.Products;
using ECommerce.Dashboard.Results;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Dashboard.Controllers;

[Authorize]
[TypeFilter<HandleApi401Exception>]
public class ProductsController(ProductService productService, CategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string? searchTerm = null, string? category = null)
    {
        var productTask = productService.GetProducts(searchTerm, new PaginationQuery(15, page), category);
        var categoryTask = categoryService.GetCategories();

        await Task.WhenAll(categoryTask, productTask);

        var products = await productTask;
        var categories = await categoryTask;
        
        var viewModel = new ProductListViewModel
        {
            Products = products.Items,
            CategorySelect = new SelectList(categories, "Slug", "Name"),
            SearchTerm = searchTerm,
            Page = products.Page,
            TotalPages = products.TotalPages,
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await productService.GetProductById(id);
        if (result.IsSuccess)
            return View(result.Value);

        return RedirectToAction(nameof(NotFound));
    }

    public async Task<IActionResult> Create()
    {
        var categoryList = await categoryService.GetCategories();

        var viewModel = new ProductCreateViewModel
        {
            CategoriesSelect = new SelectList(categoryList, "Slug", "Name")
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel model)
    {
        var categoryList = await categoryService.GetCategories();
       
        model.CategoriesSelect = new SelectList(categoryList, "Slug", "Name");
        
        if (!ModelState.IsValid)
            return View(model);

        var request = new ProductCreate
        {
            Sku = model.Sku,
            Name = model.Name,
            Category = model.Category,
            Description = model.Description,
            Price = model.Price,
            ImageFile = model.ImageFile,
            InitialStock = model.InitialStock
        };

        var result = await productService.CreateProduct(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Product created successfully";
            return RedirectToAction(nameof(Index));
        }
        
        var responseError = result.Error as ApiFailureResponseError;
        var errorMessage = responseError?.ErrorBody.Message;
        TempData["Error"] = errorMessage;

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var categoryListTask = categoryService.GetCategories();
        var productTask = productService.GetProductById(id);

        await Task.WhenAll(categoryListTask, productTask);

        var categoryList = await categoryListTask;
        var productResult = await productTask;
        
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
            CategoriesSelect = new SelectList(categoryList, "Slug", "Name", product.Category?.Slug)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductUpdateViewModel model)
    {
        var categoryList = await categoryService.GetCategories();
      
        model.CategoriesSelect = new SelectList(categoryList, "Slug", "Name", model.Category);

        if (!ModelState.IsValid)
            return View(model);

        var request = new ProductUpdate
        {
            Name = model.Name,
            Category = model.Category,
            Description = model.Description,
            Price = model.Price,
            ImageFile = model.ImageFile
        };

        var result = await productService.UpdateProduct(id, request);
        if (result.IsSuccess)
        {
            TempData["Success"] = "Product updated successfully";
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