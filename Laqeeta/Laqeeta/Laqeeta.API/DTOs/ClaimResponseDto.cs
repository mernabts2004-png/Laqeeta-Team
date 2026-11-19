namespace Laqeeta.API.DTOs
{
    public class ClaimResponseDto
    {
        public int ClaimId { get; set; }

        public int ReportId { get; set; }

        public int ItemId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? HandedOverAt { get; set; }
    }
}