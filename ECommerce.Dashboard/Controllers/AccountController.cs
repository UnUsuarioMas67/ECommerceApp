using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Services;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

public class AccountController(ApiAuthService apiAuthService, SiteAuthService siteAuthService) : Controller
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

        var loginResult = await apiAuthService.LoginAsync(request);
        if (!loginResult.IsSuccess)
            return View();

        var login = loginResult.Value;
        await siteAuthService.SignInAsync(login.User, login.AccessToken, login.RefreshToken);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await apiAuthService.LogoutAsync();
        await siteAuthService.SignOutAsync();
        
        return RedirectToAction(nameof(Login));
    }
}