using System.ComponentModel.DataAnnotations;

namespace Laqeeta.API.DTOs
{
    public class GetClaimStatusDto
    {
        [Range(1, int.MaxValue)]
        public int ClaimId { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}