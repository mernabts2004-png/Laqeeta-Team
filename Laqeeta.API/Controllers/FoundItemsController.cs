using Laqeeta.API.Data;
using Laqeeta.API.DTOs;
using Laqeeta.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Laqeeta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff")]
    public class FoundItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private static readonly TimeZoneInfo EgyptTimeZone =
            GetEgyptTimeZone();

        public FoundItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFoundItem(
            CreateFoundItemDto dto)
        {
            // Validate required text fields.
            if (string.IsNullOrWhiteSpace(dto.Category) ||
                string.IsNullOrWhiteSpace(dto.Color) ||
                string.IsNullOrWhiteSpace(dto.Line) ||
                string.IsNullOrWhiteSpace(dto.Direction) ||
                string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest("Required fields cannot be empty.");
            }

            // Validate date.
            if (dto.FoundAt == default)
            {
                return BadRequest("FoundAt is required.");
            }

            // The API expects Egypt local time without a timezone offset.
            if (dto.FoundAt.Kind != DateTimeKind.Unspecified)
            {
                return BadRequest(
                    "FoundAt must be provided as Egypt local time without a timezone offset.");
            }

            var egyptNow = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                EgyptTimeZone);

            if (dto.FoundAt > egyptNow)
            {
                return BadRequest("FoundAt cannot be in the future.");
            }

            // Check that the selected station exists.
            var stationExists = await _context.Stations
                .AnyAsync(s => s.StationId == dto.FoundAtStationId);

            if (!stationExists)
            {
                return BadRequest("Invalid station ID.");
            }

            // Convert Egypt local time to UTC before saving.
            var foundAtUtc = TimeZoneInfo.ConvertTimeToUtc(
                dto.FoundAt,
                EgyptTimeZone);

            var item = new Item
            {
                Category = dto.Category.Trim(),
                Color = dto.Color.Trim(),
                Line = dto.Line.Trim(),
                Direction = dto.Direction.Trim(),
                FoundAt = foundAtUtc,
                Description = dto.Description.Trim(),
                FoundAtStationId = dto.FoundAtStationId,
                StorageLocation = "Station",
                Status = "Found"
            };

            _context.Items.Add(item);

            await _context.SaveChangesAsync();

            var response = new CreateFoundItemResponseDto
            {
                ItemId = item.ItemId,
                Status = item.Status,
                StorageLocation = item.StorageLocation,
                FoundAt = item.FoundAt
            };

            return Ok(response);
        }

        private static TimeZoneInfo GetEgyptTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    "Egypt Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    "Africa/Cairo");
            }
        }
    }
}