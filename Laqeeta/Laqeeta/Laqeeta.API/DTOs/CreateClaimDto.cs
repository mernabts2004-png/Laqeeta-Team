using System.ComponentModel.DataAnnotations;

namespace Laqeeta.API.DTOs
{
    public class CreateClaimDto
    {
        [Range(1, int.MaxValue)]
        public int ReportId { get; set; }

        [Range(1, int.MaxValue)]
        public int ItemId { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}