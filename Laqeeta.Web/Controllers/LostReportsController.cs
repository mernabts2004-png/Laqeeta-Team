using Laqeeta.Web.Helpers;
using Laqeeta.Web.Models;
using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Laqeeta.Web.Controllers;

public class LostReportsController(LaqeetaApiClient api) : AppController
{
    [HttpGet]
    public IActionResult Create() =>
        View(new CreateReportViewModel { EventAt = UiHelper.EgyptNow() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReportViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var isHighValue = LookupData.IsHighValue(model.Category);

        var request = new CreateLostReportRequest
        {
            PhoneNumber = model.PhoneNumber.Trim(),
            Category = model.Category.Trim(),
            Color = model.Color.Trim(),
            Line = model.Line.Trim(),
            Direction = model.Direction.Trim(),
            // The API expects Egypt local time without a timezone offset.
            LostAt = DateTime.SpecifyKind(model.EventAt, DateTimeKind.Unspecified),
            Description = model.Description.Trim(),
            // The secret question is only used for high-value items.
            SecretQuestion = isHighValue ? model.SecretQuestion?.Trim() : null,
            SecretAnswer = isHighValue ? model.SecretAnswer?.Trim() : null
        };

        var result = await api.CreateLostReportAsync(request);

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر حفظ البلاغ.");
            return View(model);
        }

        return RedirectToAction(nameof(Created), new { id = result.Data.ReportId });
    }

    [HttpGet]
    public IActionResult Created(int id) => View(model: id);
}
