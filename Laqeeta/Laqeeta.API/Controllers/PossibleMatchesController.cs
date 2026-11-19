using Laqeeta.API.Data;
using Laqeeta.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laqeeta.API.Helpers;

namespace Laqeeta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PossibleMatchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PossibleMatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GetPossibleMatches(
            GetPossibleMatchesDto dto)
        {
            // Find the report using its ID and owner's phone number.
            var report = await _context.Reports
                .FirstOrDefaultAsync(r =>
                    r.ReportId == dto.ReportId &&
                    r.PhoneNumber == PhoneNumberNormalizer.Normalize(dto.PhoneNumber));

            if (report == null)
            {
                return NotFound("Report not found.");
            }

            // Closed reports cannot search for matches.
            if (report.Status == "Closed")
            {
                return BadRequest("This report is closed.");
            }

            // Define the allowed matching date range.
            var startDate = report.LostAt.AddDays(-3);
            var endDate = report.LostAt.AddDays(3);

            // Search for available matching items.
            var matches = await _context.Items
                .AsNoTracking()
                .Where(i =>
                    i.Category == report.Category &&
                    i.Line == report.Line &&
                    i.Direction == report.Direction &&
                    i.FoundAt >= startDate &&
                    i.FoundAt <= endDate &&
                    i.Status == "Found")
                .Select(i => new PossibleMatchResponseDto
                {
                    ItemId = i.ItemId,
                    Category = i.Category,
                    Color = i.Color,
                    Line = i.Line,
                    Direction = i.Direction,
                    FoundAt = i.FoundAt,
                    Description = i.Description,
                    StationName = i.FoundAtStation.Name
                })
                .ToListAsync();

            // Update report status based on matching results.
            if (report.Status == "Searching" ||
                report.Status == "PossibleMatch")
            {
                report.Status = matches.Any()
                    ? "PossibleMatch"
                    : "Searching";

                await _context.SaveChangesAsync();
            }

            return Ok(matches);
        }
    }
}