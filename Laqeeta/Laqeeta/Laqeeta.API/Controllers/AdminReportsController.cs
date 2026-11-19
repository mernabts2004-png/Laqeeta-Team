using Laqeeta.API.Data;
using Laqeeta.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Laqeeta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stations")]
        public async Task<IActionResult> GetStationReport()
        {
            var report = await _context.Stations
                .AsNoTracking()
                .Select(s => new StationReportDto
                {
                    StationId = s.StationId,
                    StationName = s.Name,

                    FoundItemsCount = _context.Items.Count(i =>
                        i.FoundAtStationId == s.StationId),

                    ReturnedItemsCount = _context.Items.Count(i =>
                        i.FoundAtStationId == s.StationId &&
                        i.Status == "Returned"),

                    ReturnRate = _context.Items.Count(i =>
                        i.FoundAtStationId == s.StationId) == 0
                        ? 0
                        : (decimal)_context.Items.Count(i =>
                            i.FoundAtStationId == s.StationId &&
                            i.Status == "Returned") * 100
                            / _context.Items.Count(i =>
                                i.FoundAtStationId == s.StationId)
                })
                .ToListAsync();

            return Ok(report);
        }
    }
}