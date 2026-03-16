using Api.DTOs;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Mappers;
using Api.Models;
using Api.Models.Enums;
using Api.Services.Interfaces;

namespace Api.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
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
    }
}
