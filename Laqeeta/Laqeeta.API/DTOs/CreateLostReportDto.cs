using System.ComponentModel.DataAnnotations;

namespace Laqeeta.API.DTOs
{
    public class CreateLostReportDto
    {
        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

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
        public DateTime LostAt { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? SecretQuestion { get; set; }

        [MaxLength(200)]
        public string? SecretAnswer { get; set; }
    }
}