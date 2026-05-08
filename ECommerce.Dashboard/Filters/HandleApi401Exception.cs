using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerce.Dashboard.Filters;

public class HandleApi401Exception : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var httpRequestException = context.Exception as HttpRequestException;
        if (httpRequestException == null)
            return;

        if (httpRequestException.StatusCode != HttpStatusCode.Unauthorized)
            return;

        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Result = new RedirectToActionResult("Login", "Account", null);
    }
}