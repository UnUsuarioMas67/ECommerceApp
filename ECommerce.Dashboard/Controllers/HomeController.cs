using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Dashboard.Models;
using ECommerce.Dashboard.Services;
using ECommerce.Dashboard.Services.Api;

namespace ECommerce.Dashboard.Controllers;

public class HomeController(ILogger<HomeController> logger, AuthService authService) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    public async Task<IActionResult> Index()
    {
        var result = await authService.GetAuthenticatedUserAsync();
        if (!result.IsSuccess)
            return RedirectToAction("Login", "Account");
        
        return View(result.Value);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}