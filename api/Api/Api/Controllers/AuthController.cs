using Api.DTOs;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and account management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the AuthController
        /// </summary>
        /// <param name="userService">User service for authentication operations</param>
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registers a new user account
        /// </summary>
        /// <param name="request">Registration details including email and password</param>
        /// <returns>Authentication response with JWT token</returns>
        /// <response code="200">Returns the JWT token and user email</response>
        /// <response code="400">If the user already exists or password is invalid</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/signup
        ///     {
        ///         "email": "user@example.com",
        ///         "password": "SecurePass123"
        ///     }
        ///     
        /// Password requirements:
        /// - Minimum 8 characters
        /// - At least one uppercase letter
        /// - At least one lowercase letter
        /// - At least one digit
        /// </remarks>
        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                var response = await _userService.SignUp(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates an existing user
        /// </summary>
        /// <param name="request">Login credentials including email and password</param>
        /// <returns>Authentication response with JWT token</returns>
        /// <response code="200">Returns the JWT token and user email</response>
        /// <response code="401">If the credentials are invalid</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/signin
        ///     {
        ///         "email": "user@example.com",
        ///         "password": "SecurePass123"
        ///     }
        /// </remarks>
        [HttpPost("signin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                var response = await _userService.SignIn(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Initiates password reset process by sending a reset token to the user's email
        /// </summary>
        /// <param name="request">Request containing the user's email address</param>
        /// <returns>Success message</returns>
        /// <response code="200">Returns success message (always returns this for security)</response>
        /// <response code="500">If an unexpected error occurred</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/forgot-password
        ///     {
        ///         "email": "user@example.com"
        ///     }
        ///     
        /// Notes:
        /// - For security reasons, the API doesn't reveal whether the email exists
        /// - Reset token is valid for 1 hour
        /// - In development, the token is logged to console
        /// - In production, configure SMTP to send actual emails
        /// </remarks>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                await _userService.RequestPasswordResetAsync(request.Email);
                return Ok(new { message = "If the email exists, a password reset link has been sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Resets user password using a valid reset token
        /// </summary>
        /// <param name="request">Request containing the reset token and new password</param>
        /// <returns>Success message</returns>
        /// <response code="200">Password successfully reset</response>
        /// <response code="400">If the token is invalid/expired or password doesn't meet requirements</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/reset-password
        ///     {
        ///         "token": "base64-encoded-reset-token",
        ///         "newPassword": "NewSecurePass123"
        ///     }
        ///     
        /// Password requirements:
        /// - Minimum 8 characters
        /// - At least one uppercase letter
        /// - At least one lowercase letter
        /// - At least one digit
        /// 
        /// Token requirements:
        /// - Must be unused
        /// - Must not be expired (valid for 1 hour)
        /// </remarks>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _userService.ResetPasswordAsync(request.Token, request.NewPassword);
                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Logs out the current user by invalidating their JWT token
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">User successfully logged out</response>
        /// <response code="401">If the token is missing or invalid</response>
        /// <response code="500">If an unexpected error occurred</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/logout
        ///     Headers: Authorization: Bearer {your-jwt-token}
        ///     
        /// Notes:
        /// - Requires valid JWT token in Authorization header
        /// - Token is added to blacklist and cannot be reused
        /// - Token remains blacklisted until its natural expiration
        /// </remarks>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                await _userService.LogoutAsync(token);
                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while logging out." });
            }
        }
    }
}
