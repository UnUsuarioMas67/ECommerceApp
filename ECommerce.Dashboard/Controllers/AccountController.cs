using ECommerce.Dashboard.Models;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Services;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

public class AccountController(AuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        var model = new LoginViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var loginResult = await apiAuthService.LoginAsync(model.LoginRequest);
        if (!loginResult.IsSuccess)
            return View(model);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        return RedirectToAction(nameof(Login));
    }
}