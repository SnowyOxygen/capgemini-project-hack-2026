using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// Request model for initiating password reset
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// Email address of the account to reset password for
        /// </summary>
        /// <example>user@example.com</example>
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
