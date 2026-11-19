namespace Laqeeta.API.DTOs
{
    public class ReportStatusResponseDto
    {
        public int ReportId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}