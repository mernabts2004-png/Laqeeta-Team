using Laqeeta.Web.Helpers;
using Laqeeta.Web.Models;
using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laqeeta.Web.Controllers;

[Authorize(Roles = "Staff")]
public class StaffController(LaqeetaApiClient api) : AppController
{
    public IActionResult Index() => View();

    // ---------- Log a found item ----------

    [HttpGet]
    public IActionResult FoundItem() =>
        View(new FoundItemViewModel { EventAt = UiHelper.EgyptNow() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FoundItem(FoundItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new CreateFoundItemRequest
        {
            Category = model.Category.Trim(),
            Color = model.Color.Trim(),
            Line = model.Line.Trim(),
            Direction = model.Direction.Trim(),
            // The API expects Egypt local time without a timezone offset.
            FoundAt = DateTime.SpecifyKind(model.EventAt, DateTimeKind.Unspecified),
            Description = model.Description.Trim(),
            FoundAtStationId = model.StationId!.Value
        };

        var result = await api.CreateFoundItemAsync(request);

        var session = await CheckSessionAsync(result);

        if (session is not null)
        {
            return session;
        }

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر تسجيل الغرض.");
            return View(model);
        }

        TempData["Success"] =
            $"تم تسجيل الغرض برقم {result.Data.ItemId}، وهو محفوظ في المحطة ({result.Data.Status}).";

        return RedirectToAction(nameof(FoundItem));
    }

    // ---------- Handle a claim ----------

    // GET /Staff/ManageClaim            -> asks for the claim number
    // GET /Staff/ManageClaim?id=5       -> shows the actions for claim 5
    [HttpGet]
    public async Task<IActionResult> ManageClaim(int? id)
    {
        if (id is null)
        {
            return View(model: null as StaffClaimViewModel);
        }

        var question = await api.GetSecretQuestionAsync(id.Value);

        var session = await CheckSessionAsync(question);

        if (session is not null)
        {
            return session;
        }

        // 404 means that the claim number does not exist.
        if (question.Status == System.Net.HttpStatusCode.NotFound)
        {
            TempData["Error"] = question.Error;
            return View(model: null as StaffClaimViewModel);
        }

        var model = new StaffClaimViewModel
        {
            ClaimId = id.Value,
            SecretQuestion = question.Success ? question.Data?.SecretQuestion : null,
            Note = question.Success ? null : question.Error
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(int id, bool idVerified, string? secretAnswer)
    {
        var result = await api.VerifyClaimAsync(id, idVerified, secretAnswer?.Trim());

        return await FinishActionAsync(
            result,
            id,
            "تم التحقق من المالك بنجاح. تقدر دلوقتي توافق على الطلب.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await api.ApproveClaimAsync(id);

        return await FinishActionAsync(
            result,
            id,
            "تمت الموافقة. الغرض اتحجز وجاهز للتسليم للمالك.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await api.RejectClaimAsync(id);

        return await FinishActionAsync(result, id, "تم رفض الطلب.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Handover(int id)
    {
        var result = await api.HandoverClaimAsync(id);

        return await FinishActionAsync(
            result,
            id,
            "تم تسليم الغرض للمالك وقفل البلاغ.");
    }

    private async Task<IActionResult> FinishActionAsync(
        ApiResult result,
        int claimId,
        string successMessage)
    {
        var session = await CheckSessionAsync(result);

        if (session is not null)
        {
            return session;
        }

        if (result.Success)
        {
            TempData["Success"] = successMessage;
        }
        else
        {
            TempData["Error"] = result.Error;
        }

        return RedirectToAction(nameof(ManageClaim), new { id = claimId });
    }
}
