namespace Laqeeta.API.Models
{
    public class Report
    {
        public int ReportId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string Line { get; set; } = string.Empty;

        public string Direction { get; set; } = string.Empty;

        public DateTime LostAt { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Searching";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ownership verification for high-value items
        public string? SecretQuestion { get; set; }

        public string? SecretAnswerHash { get; set; }

        // Navigation Property
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}