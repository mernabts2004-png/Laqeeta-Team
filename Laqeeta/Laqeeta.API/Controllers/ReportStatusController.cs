using Laqeeta.API.Data;
using Laqeeta.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laqeeta.API.Helpers;

namespace Laqeeta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GetReportStatus(
    GetReportStatusDto dto)
        {
            var phoneNumber = PhoneNumberNormalizer.Normalize(dto.PhoneNumber);

            var report = await _context.Reports
                .FirstOrDefaultAsync(r =>
                    r.ReportId == dto.ReportId &&
                    r.PhoneNumber == phoneNumber);

            if (report == null)
            {
                return NotFound("Report not found.");
            }

            // Refresh the status if the report is still waiting for a match.
            if (report.Status == "Searching" ||
                report.Status == "PossibleMatch")
            {
                var startDate = report.LostAt.AddDays(-3);
                var endDate = report.LostAt.AddDays(3);

                var hasMatches = await _context.Items
                    .AnyAsync(i =>
                        i.Category == report.Category &&
                        i.Line == report.Line &&
                        i.Direction == report.Direction &&
                        i.FoundAt >= startDate &&
                        i.FoundAt <= endDate &&
                        i.Status == "Found");

                var newStatus = hasMatches ? "PossibleMatch" : "Searching";

                if (report.Status != newStatus)
                {
                    report.Status = newStatus;
                    await _context.SaveChangesAsync();
                }
            }

            var response = new ReportStatusResponseDto
            {
                ReportId = report.ReportId,
                Status = report.Status,
                CreatedAt = report.CreatedAt
            };

            return Ok(response);
        }



    }
}