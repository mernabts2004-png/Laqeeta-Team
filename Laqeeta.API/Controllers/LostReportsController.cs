using Laqeeta.API.Data;
using Laqeeta.API.DTOs;
using Laqeeta.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laqeeta.API.Helpers;

namespace Laqeeta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LostReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private static readonly TimeZoneInfo EgyptTimeZone =
            GetEgyptTimeZone();

        public LostReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLostReport(
            CreateLostReportDto dto)
        {
            // Validate required text fields.
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Category) ||
                string.IsNullOrWhiteSpace(dto.Color) ||
                string.IsNullOrWhiteSpace(dto.Line) ||
                string.IsNullOrWhiteSpace(dto.Direction) ||
                string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest("Required fields cannot be empty.");
            }

            // Validate date.
            if (dto.LostAt == default)
            {
                return BadRequest("LostAt is required.");
            }

            // The API expects Egypt local time without a timezone offset.
            if (dto.LostAt.Kind != DateTimeKind.Unspecified)
            {
                return BadRequest(
                    "LostAt must be provided as Egypt local time without a timezone offset.");
            }

            var egyptNow = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                EgyptTimeZone);

            if (dto.LostAt > egyptNow)
            {
                return BadRequest("LostAt cannot be in the future.");
            }

            // Validate secret question and answer.
            bool hasQuestion =
                !string.IsNullOrWhiteSpace(dto.SecretQuestion);

            bool hasAnswer =
                !string.IsNullOrWhiteSpace(dto.SecretAnswer);

            if (hasQuestion != hasAnswer)
            {
                return BadRequest(
                    "Secret question and answer must be provided together.");
            }

            // High-value categories require ownership verification.
            var categoryWords = dto.Category
                .Trim()
                .ToLowerInvariant()
                .Split(
                    new[] { ' ', '-', '_' },
                    StringSplitOptions.RemoveEmptyEntries);

            bool isHighValue =
                categoryWords.Any(word =>
                    word == "phone" ||
                    word == "mobile" ||
                    word == "smartphone" ||
                    word == "wallet" ||
                    word == "id" ||
                    word == "identity");

            if (isHighValue && (!hasQuestion || !hasAnswer))
            {
                return BadRequest(
                    "Secret question and answer are required for high-value items.");
            }

            // Convert Egypt local time to UTC before saving.
            var lostAtUtc = TimeZoneInfo.ConvertTimeToUtc(
                dto.LostAt,
                EgyptTimeZone);

            var report = new Report
            {
                PhoneNumber = PhoneNumberNormalizer.Normalize(dto.PhoneNumber),
                Category = dto.Category.Trim(),
                Color = dto.Color.Trim(),
                Line = dto.Line.Trim(),
                Direction = dto.Direction.Trim(),
                LostAt = lostAtUtc,
                Description = dto.Description.Trim(),
                Status = "Searching",
                CreatedAt = DateTime.UtcNow,
                SecretQuestion = hasQuestion
                    ? dto.SecretQuestion!.Trim()
                    : null
            };

            // Hash the secret answer instead of storing plain text.
            if (hasAnswer)
            {
                var passwordHasher = new PasswordHasher<Report>();

                report.SecretAnswerHash =
                passwordHasher.HashPassword(
                report,
                SecretAnswerNormalizer.Normalize(dto.SecretAnswer!));
            }

            _context.Reports.Add(report);

            await _context.SaveChangesAsync();

            var response = new CreateLostReportResponseDto
            {
                ReportId = report.ReportId,
                Status = report.Status,
                CreatedAt = report.CreatedAt
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