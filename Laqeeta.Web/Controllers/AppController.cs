using System.Net;
using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Laqeeta.Web.Controllers;

public abstract class AppController : Controller
{
    // For staff / admin calls: if the API says the token is expired or the role
    // is wrong, send the user to the right page. Returns null when all is fine.
    protected async Task<IActionResult?> CheckSessionAsync(ApiResult result)
    {
        if (result.Status == HttpStatusCode.Unauthorized)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Error"] = "انتهت الجلسة. سجّل دخول تاني.";

            return RedirectToAction(
                "Login",
                "Account",
                new { returnUrl = Request.Path + Request.QueryString });
        }

        if (result.Status == HttpStatusCode.Forbidden)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        return null;
    }
}
