using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    /// <summary>
    /// Request model for resetting password with token
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Password reset token received via email
        /// </summary>
        /// <example>base64-encoded-token-string</example>
        [Required]
        public required string Token { get; set; }

        /// <summary>
        /// New password (min 8 chars, must contain uppercase, lowercase, and digit)
        /// </summary>
        /// <example>NewSecurePass123</example>
        [Required]
        [MinLength(8)]
        public required string NewPassword { get; set; }
    }
}
