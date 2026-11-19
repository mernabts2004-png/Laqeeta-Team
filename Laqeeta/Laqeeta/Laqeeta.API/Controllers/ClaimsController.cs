using Laqeeta.API.Data;
using Laqeeta.API.DTOs;
using Laqeeta.API.Helpers;
using Laqeeta.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Laqeeta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Passenger creates a claim
        [HttpPost]
        public async Task<IActionResult> CreateClaim(CreateClaimDto dto)
        {
            var phoneNumber = PhoneNumberNormalizer.Normalize(dto.PhoneNumber);

            var report = await _context.Reports
                .FirstOrDefaultAsync(r =>
                    r.ReportId == dto.ReportId &&
                    r.PhoneNumber == phoneNumber);

            if (report == null)
                return NotFound("Report not found.");

            if (report.Status == "Closed")
                return BadRequest("This report is closed.");

            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.ItemId == dto.ItemId);

            if (item == null)
                return NotFound("Item not found.");

            if (item.Status != "Found")
                return BadRequest("This item is not available for claims.");

            // Ensure that the item matches the lost report.
            var startDate = report.LostAt.AddDays(-3);
            var endDate = report.LostAt.AddDays(3);

            var isMatchingItem =
                item.Category == report.Category &&
                item.Line == report.Line &&
                item.Direction == report.Direction &&
                item.FoundAt >= startDate &&
                item.FoundAt <= endDate;

            if (!isMatchingItem)
                return BadRequest("This item does not match the lost report.");

            var isDuplicate = await _context.Claims.AnyAsync(c =>
                c.ReportId == dto.ReportId &&
                c.ItemId == dto.ItemId);

            if (isDuplicate)
                return BadRequest("This claim already exists.");

            var claim = new Claim
            {
                ReportId = report.ReportId,
                ItemId = item.ItemId,
                Status = "Pending",
                IdVerified = false,
                SecretQuestionVerified = false
            };

            _context.Claims.Add(claim);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (GetSqlErrorNumber(ex) is 2601 or 2627)
            {
                // The same claim was sent twice at the same time.
                return BadRequest("This claim already exists.");
            }

            return Ok(new ClaimResponseDto
            {
                ClaimId = claim.ClaimId,
                ReportId = claim.ReportId,
                ItemId = claim.ItemId,
                Status = claim.Status,
                HandedOverAt = claim.HandedOverAt
            });
        }

        // 2. Passenger checks claim status
        [HttpPost("status")]
        public async Task<IActionResult> GetClaimStatus(
            GetClaimStatusDto dto)
        {
            var phoneNumber = PhoneNumberNormalizer.Normalize(dto.PhoneNumber);

            var claim = await _context.Claims
                .AsNoTracking()
                .Include(c => c.Report)
                .FirstOrDefaultAsync(c =>
                    c.ClaimId == dto.ClaimId &&
                    c.Report.PhoneNumber == phoneNumber);

            if (claim == null)
                return NotFound("Claim not found.");

            return Ok(new ClaimResponseDto
            {
                ClaimId = claim.ClaimId,
                ReportId = claim.ReportId,
                ItemId = claim.ItemId,
                Status = claim.Status,
                HandedOverAt = claim.HandedOverAt
            });
        }

        // 3. Staff retrieves the secret question
        [Authorize(Roles = "Staff")]
        [HttpGet("{claimId}/secret-question")]
        public async Task<IActionResult> GetSecretQuestion(int claimId)
        {
            var claim = await _context.Claims
                .AsNoTracking()
                .Include(c => c.Report)
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null)
                return NotFound("Claim not found.");

            if (claim.Status != "Pending")
                return BadRequest("Only pending claims can be verified.");

            if (string.IsNullOrWhiteSpace(claim.Report.SecretQuestion))
                return BadRequest("No secret question is configured for this report.");

            return Ok(new
            {
                ClaimId = claim.ClaimId,
                SecretQuestion = claim.Report.SecretQuestion
            });
        }

        // 3. Staff verifies ownership
        [Authorize(Roles = "Staff")]
        [HttpPost("{claimId}/verify")]
        public async Task<IActionResult> VerifyClaim(
            int claimId,
            VerifyClaimDto dto)
        {
            var claim = await _context.Claims
                .Include(c => c.Report)
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null)
                return NotFound("Claim not found.");

            if (claim.Status != "Pending")
                return BadRequest("Only pending claims can be verified.");

            if (!dto.IdVerified)
                return BadRequest("ID verification failed.");

            var requiresSecretAnswer =
                IsHighValueCategory(claim.Report.Category);

            if (requiresSecretAnswer)
            {
                if (string.IsNullOrWhiteSpace(dto.SecretAnswer))
                    return BadRequest("Secret answer is required.");

                if (string.IsNullOrWhiteSpace(claim.Report.SecretAnswerHash))
                    return BadRequest("Secret answer is not configured.");


                var passwordHasher = new PasswordHasher<Report>();

                var normalizedResult = passwordHasher.VerifyHashedPassword(
                    claim.Report,
                    claim.Report.SecretAnswerHash,
                    SecretAnswerNormalizer.Normalize(dto.SecretAnswer));

                // Old reports were hashed with Trim() only, so we also accept that form.
                var legacyResult = passwordHasher.VerifyHashedPassword(
                    claim.Report,
                    claim.Report.SecretAnswerHash,
                    dto.SecretAnswer.Trim());

                if (normalizedResult == PasswordVerificationResult.Failed &&
                    legacyResult == PasswordVerificationResult.Failed)
                    return BadRequest("Incorrect secret answer.");

            }

            claim.IdVerified = true;
            claim.SecretQuestionVerified = true;
            claim.VerifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Ownership verification completed successfully.");
        }

        // 4. Staff approves the claim
        [Authorize(Roles = "Staff")]
        [HttpPost("{claimId}/approve")]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            try
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(
                        IsolationLevel.Serializable);

                var claim = await _context.Claims
                    .Include(c => c.Report)
                    .Include(c => c.Item)
                    .FirstOrDefaultAsync(c => c.ClaimId == claimId);

                if (claim == null)
                    return NotFound("Claim not found.");

                if (claim.Status != "Pending")
                    return BadRequest("Only pending claims can be approved.");

                if (!claim.IdVerified || !claim.SecretQuestionVerified)
                    return BadRequest("Ownership verification is required.");

                if (claim.Item.Status != "Found")
                    return BadRequest("This item is no longer available.");

                var anotherApprovedClaim = await _context.Claims.AnyAsync(c =>
                    c.ItemId == claim.ItemId &&
                    (c.Status == "Approved" || c.Status == "Completed"));

                if (anotherApprovedClaim)
                    return BadRequest("This item has already been claimed.");

                claim.Status = "Approved";
                claim.Item.Status = "Reserved";
                claim.Report.Status = "ReadyToCollect";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Claim approved. The item is ready for collection.");
            }
            catch (Exception ex) when (GetSqlErrorNumber(ex) == 1205)
            {
                // SQL Server stopped this request because two approvals collided.
                return BadRequest(
                    "Another staff member is working on this item. Please try again.");
            }
        }

        // 5. Staff rejects the claim
        [Authorize(Roles = "Staff")]
        [HttpPost("{claimId}/reject")]
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null)
                return NotFound("Claim not found.");

            if (claim.Status != "Pending")
                return BadRequest("Only pending claims can be rejected.");

            claim.Status = "Rejected";

            await _context.SaveChangesAsync();

            return Ok("Claim rejected.");
        }

        // 6. Staff completes the handover
        [Authorize(Roles = "Staff")]
        [HttpPost("{claimId}/handover")]
        public async Task<IActionResult> CompleteHandover(int claimId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var claim = await _context.Claims
                .Include(c => c.Report)
                .Include(c => c.Item)
                .FirstOrDefaultAsync(c => c.ClaimId == claimId);

            if (claim == null)
                return NotFound("Claim not found.");

            if (claim.Status != "Approved")
                return BadRequest("Only approved claims can be handed over.");

            if (claim.Item.Status != "Reserved")
                return BadRequest("This item is not reserved for this claim.");

            claim.Status = "Completed";
            claim.HandedOverAt = DateTime.UtcNow;

            claim.Item.Status = "Returned";
            claim.Report.Status = "Closed";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new ClaimResponseDto
            {
                ClaimId = claim.ClaimId,
                ReportId = claim.ReportId,
                ItemId = claim.ItemId,
                Status = claim.Status,
                HandedOverAt = claim.HandedOverAt
            });
        }

        private static bool IsHighValueCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return false;
            }

            var highValueCategories = new[]
            {
        "phone",
        "mobile",
        "smartphone",
        "wallet",
        "id",
        "identity"
    };

            var categoryWords = category
                .Trim()
                .ToLowerInvariant()
                .Split(
                    new[] { ' ', '-', '_' },
                    StringSplitOptions.RemoveEmptyEntries
                );

            return categoryWords.Any(word =>
                highValueCategories.Contains(word));
        }

        // Returns the SQL Server error number behind an exception, if any.
        private static int? GetSqlErrorNumber(Exception ex)
        {
            var sqlException = ex as SqlException
                ?? ex.InnerException as SqlException;

            return sqlException?.Number;
        }
    }
}