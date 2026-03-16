using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class SignInRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
