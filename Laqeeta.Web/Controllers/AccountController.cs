using System.Security.Claims;
using Laqeeta.Web.Models;
using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Laqeeta.Web.Controllers;

public class AccountController(LaqeetaApiClient api) : AppController
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole();
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await api.LoginAsync(model.Email.Trim(), model.Password);

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر تسجيل الدخول.");
            return View(model);
        }

        var login = result.Data;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(login.FullName)
                ? model.Email.Trim()
                : login.FullName),
            new(ClaimTypes.Email, model.Email.Trim())
        };

        foreach (var role in login.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            IsPersistent = false,
            AllowRefresh = false,
            // The cookie dies at the same time as the API token.
            ExpiresUtc = login.ExpiresAt.UtcDateTime
        };

        properties.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "access_token", Value = login.Token }
        });

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        if (login.Roles.Contains("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }

        if (login.Roles.Contains("Staff"))
        {
            return RedirectToAction("Index", "Staff");
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    private IActionResult RedirectByRole()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }

        if (User.IsInRole("Staff"))
        {
            return RedirectToAction("Index", "Staff");
        }

        return RedirectToAction("Index", "Home");
    }
}
