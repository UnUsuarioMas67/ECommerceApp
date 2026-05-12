using System.Security.Claims;
using ECommerce.Dashboard.DTOs.Auth;
using ECommerce.Dashboard.DTOs.User;
using ECommerce.Dashboard.Filters;
using ECommerce.Dashboard.Models.Account;
using ECommerce.Dashboard.Services;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

public class AccountController(AuthService authService, CookieHelperService cookieHelperService) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        var model = new LoginModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
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

        await SignInPrincipal(user);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        cookieHelperService.DeleteApiTokenCookies(HttpContext);

        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [TypeFilter<HandleApi401Exception>]
    public async Task<IActionResult> Settings([FromServices] UserService userService)
    {
        var user = await userService.GetCurrent();

        var viewModel = new UserUpdateModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            BirthDate = user.BirthDate,
        };

        return View(viewModel);
    }

    [Authorize]
    [TypeFilter<HandleApi401Exception>]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUser([FromServices] UserService userService, UserUpdateModel model)
    {
        var request = new AdminUserUpdate
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            BirthDate = model.BirthDate.ToString("yyyy-MM-dd"),
        };

        var updateResult = await userService.UpdateUser(request);
        if (!updateResult.IsSuccess)
        {
            TempData["Error"] = updateResult.Error.Message;
            return View(nameof(Settings), model);
        }

        await SignInPrincipal(updateResult.Value);

        TempData["Success"] = "User successfully updated";
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInPrincipal(AdminUserResponse user)
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
}