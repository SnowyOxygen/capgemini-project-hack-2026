namespace Api.DTOs
{
    /// <summary>
    /// Response model for authentication operations
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT token for authenticated requests
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// User's email address
        /// </summary>
        public required string Email { get; set; }
    }
}
