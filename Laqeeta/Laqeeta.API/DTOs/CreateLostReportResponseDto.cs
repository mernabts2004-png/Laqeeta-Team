namespace Laqeeta.API.DTOs
{
    public class CreateLostReportResponseDto
    {
        public int ReportId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}