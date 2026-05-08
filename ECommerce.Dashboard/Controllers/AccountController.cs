using System.Globalization;
using System.Security.Claims;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.Models.Auth;
using ECommerce.Dashboard.Services;
using ECommerce.Dashboard.Services.Api;
using ECommerce.Dashboard.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard.Controllers;

public class AccountController(AuthService authService, IOptions<AuthSettings> options) : Controller
{
    private readonly AuthSettings _authSettings = options.Value;
    
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

        var loginResult = await authService.LoginAsync(model.LoginRequest);
        if (!loginResult.IsSuccess)
            return View(model);

        var login = loginResult.Value;
        SetApiTokenCookies(login);
        await SignInAsync(login.User);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        DeleteApiTokenCookies();
        
        return RedirectToAction(nameof(Login));
    }
 
    private void SetApiTokenCookies(UserLoginResponse login)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddDays(5),
            Secure = true,
            SameSite = SameSiteMode.Lax
        };

        HttpContext.Response.Cookies.Append(_authSettings.JwtCookieKey, login.AccessToken, cookieOptions);
        HttpContext.Response.Cookies.Append(_authSettings.RefreshCookieKey, login.RefreshToken, cookieOptions);
        HttpContext.Response.Cookies.Append(_authSettings.JwtExpireCookieKey,
            login.ExpiresAt.ToString("O", CultureInfo.InvariantCulture), cookieOptions);
    }
    
    private async Task SignInAsync(AdminUser user)
    {
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("yyyy-MM-dd")),
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(principal);
    }
    
    private void DeleteApiTokenCookies()
    {
        HttpContext.Response.Cookies.Delete(_authSettings.JwtCookieKey);
        HttpContext.Response.Cookies.Delete(_authSettings.RefreshCookieKey);
        HttpContext.Response.Cookies.Delete(_authSettings.JwtExpireCookieKey);
    }
}