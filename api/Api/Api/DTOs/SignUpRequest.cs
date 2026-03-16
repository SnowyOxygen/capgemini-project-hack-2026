using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// Request model for user registration
    /// </summary>
    public class SignUpRequest
    {
        /// <summary>
        /// User's email address (must be unique)
        /// </summary>
        /// <example>newuser@example.com</example>
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        /// <summary>
        /// User's password (min 8 chars, must contain uppercase, lowercase, and digit)
        /// </summary>
        /// <example>SecurePass123</example>
        [Required]
        public required string Password { get; set; }
    }
}
