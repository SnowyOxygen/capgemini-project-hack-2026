using Api.DTOs;
using Api.Models;

namespace Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmail(string email);
        Task<AuthResponse> SignUp(SignUpRequest request);
        Task<AuthResponse> SignIn(SignInRequest request);
    }
}
