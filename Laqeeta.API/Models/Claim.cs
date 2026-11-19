namespace Laqeeta.API.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        public int ReportId { get; set; }

        public Report Report { get; set; } = null!;

        public int ItemId { get; set; }
        
        public Item Item { get; set; } = null!;

        public string Status { get; set; } = "Pending";

        public bool IdVerified { get; set; }

        public bool SecretQuestionVerified { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public DateTime? HandedOverAt { get; set; }
    }

}
