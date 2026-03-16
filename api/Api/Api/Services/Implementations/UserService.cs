using Api.DTOs;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Mappers;
using Api.Models;
using Api.Models.Enums;
using Api.Services.Interfaces;
using System.Security.Cryptography;

namespace Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IEmailService _emailService;
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IEmailService emailService,
            ITokenBlacklistService tokenBlacklistService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _emailService = emailService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<AuthResponse> SignUp(SignUpRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmail(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            if (!_passwordService.ValidatePassword(request.Password))
            {
                throw new InvalidOperationException(
                    "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit");
            }

            var hashedPassword = _passwordService.HashPassword(request.Password);
            var user = request.ToUser(hashedPassword, (int)UserRole.User);

            await _userRepository.CreateUser(user);

            var token = _jwtService.GenerateToken(user.Email, "User");

            return new AuthResponse
            {
                Token = token,
                Email = user.Email
            };
        }

        public async Task<AuthResponse> SignIn(SignInRequest request)
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (!_passwordService.VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var roleName = user.Role?.Name ?? "User";
            var token = _jwtService.GenerateToken(user.Email, roleName);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email
            };
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                // Don't reveal if user exists or not for security reasons
                return;
            }

            // Generate a secure random token
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var resetToken = Convert.ToBase64String(tokenBytes);

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.Id!.Value,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                IsUsed = false
            };

            await _passwordResetTokenRepository.CreateTokenAsync(passwordResetToken);
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
        }

        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            var resetToken = await _passwordResetTokenRepository.GetValidTokenAsync(token);
            if (resetToken == null)
            {
                throw new InvalidOperationException("Invalid or expired reset token");
            }

            if (!_passwordService.ValidatePassword(newPassword))
            {
                throw new InvalidOperationException(
                    "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit");
            }

            var user = resetToken.User;
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            user.Password = _passwordService.HashPassword(newPassword);
            await _userRepository.UpdateUser(user);
            await _passwordResetTokenRepository.MarkTokenAsUsedAsync(resetToken.Id);
        }

        public async Task LogoutAsync(string token)
        {
            // Extract expiration time from JWT token
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiresAt = jwtToken.ValidTo;

            await _tokenBlacklistService.BlacklistTokenAsync(token, expiresAt);
        }
    }
}
