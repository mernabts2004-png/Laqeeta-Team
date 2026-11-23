using Laqeeta.Web.Models;
using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Laqeeta.Web.Controllers;

public class TrackController(LaqeetaApiClient api) : AppController
{
    [HttpGet]
    public IActionResult Index(int? reportId = null)
    {
        var model = new TrackRequestViewModel();

        if (reportId.HasValue)
        {
            model.ReportId = reportId.Value;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(TrackRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var phone = model.PhoneNumber.Trim();

        // ReportStatus also refreshes Searching / PossibleMatch on the API side.
        var status = await api.GetReportStatusAsync(model.ReportId, phone);

        if (!status.Success || status.Data is null)
        {
            ModelState.AddModelError(string.Empty, status.Error ?? "تعذر الاستعلام عن البلاغ.");
            return View(model);
        }

        var result = new TrackResultViewModel
        {
            ReportId = status.Data.ReportId,
            PhoneNumber = phone,
            Status = status.Data.Status,
            CreatedAt = status.Data.CreatedAt
        };

        // Closed reports cannot search for matches (the API returns 400).
        if (result.Status != "Closed")
        {
            var matches = await api.GetPossibleMatchesAsync(model.ReportId, phone);

            if (matches.Success && matches.Data is not null)
            {
                result.Matches = matches.Data;
            }
            else
            {
                result.MatchesError = matches.Error;
            }
        }

        return View("Result", result);
    }
}
