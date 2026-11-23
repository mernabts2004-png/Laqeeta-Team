using Laqeeta.Web.Models;
using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Laqeeta.Web.Controllers;

// Passenger side of claims (no login). Staff actions live in StaffController.
public class ClaimsController(LaqeetaApiClient api) : AppController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int reportId, int itemId, string phoneNumber)
    {
        var result = await api.CreateClaimAsync(reportId, itemId, phoneNumber);

        var model = new ClaimResultViewModel
        {
            Claim = result.Data,
            Error = result.Success ? null : result.Error,
            JustCreated = result.Success
        };

        return View("Result", model);
    }

    [HttpGet]
    public IActionResult Status(int? claimId = null)
    {
        var model = new ClaimStatusLookupViewModel();

        if (claimId.HasValue)
        {
            model.ClaimId = claimId.Value;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Status(ClaimStatusLookupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await api.GetClaimStatusAsync(model.ClaimId, model.PhoneNumber.Trim());

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر الاستعلام عن الطلب.");
            return View(model);
        }

        return View("Result", new ClaimResultViewModel { Claim = result.Data });
    }
}
