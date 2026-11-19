using System.ComponentModel.DataAnnotations;

namespace Laqeeta.API.DTOs
{
    public class CreateFoundItemDto
    {
        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Line { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Direction { get; set; } = string.Empty;

        [Required]
        public DateTime FoundAt { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int FoundAtStationId { get; set; }
    }
}