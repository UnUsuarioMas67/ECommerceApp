using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

public class AccountController(AuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        var request = new UserLoginRequest();
        return View(request);
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var loginResult = await authService.LoginAsync(request);
        if (!loginResult.IsSuccess)
            return View();
        
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        return RedirectToAction(nameof(Login));
    }
}