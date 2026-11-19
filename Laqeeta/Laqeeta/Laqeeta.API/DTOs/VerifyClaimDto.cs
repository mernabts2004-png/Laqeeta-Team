using System.ComponentModel.DataAnnotations;

namespace Laqeeta.API.DTOs
{
    public class VerifyClaimDto
    {
        public bool IdVerified { get; set; }

        [MaxLength(200)]
        public string? SecretAnswer { get; set; }
    }
}