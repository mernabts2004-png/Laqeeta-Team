using System.ComponentModel.DataAnnotations;

namespace Laqeeta.API.DTOs
{
    public class GetPossibleMatchesDto
    {
        [Range(1, int.MaxValue)]
        public int ReportId { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}