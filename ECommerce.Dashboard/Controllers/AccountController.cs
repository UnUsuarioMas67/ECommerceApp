using System.Security.Claims;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Models.Account;
using ECommerce.Dashboard.Services;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

public class AccountController(AuthService authService, CookieHelperService cookieHelperService) : Controller
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

        var request = new UserLoginRequest
        {
            Email = model.Email,
            Password = model.Password,
        };

        var loginResult = await authService.LoginAsync(request);
        if (!loginResult.IsSuccess)
        {
            TempData["Error"] = loginResult.Error.Message;
            return View(model);
        }

        var login = loginResult.Value;
        
        cookieHelperService.SetApiTokenCookies(login, HttpContext);
        
        var user = login.User;
        
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("yyyy-MM-dd")),
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(principal);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        cookieHelperService.DeleteApiTokenCookies(HttpContext);
        
        return RedirectToAction(nameof(Login));
    }
}