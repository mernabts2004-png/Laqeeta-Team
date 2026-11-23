using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laqeeta.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(LaqeetaApiClient api) : AppController
{
    public async Task<IActionResult> Index()
    {
        var result = await api.GetStationReportAsync();

        var session = await CheckSessionAsync(result);

        if (session is not null)
        {
            return session;
        }

        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "تعذر تحميل التقرير.";
            return View(new List<Models.StationReportResponse>());
        }

        return View(result.Data);
    }
}
