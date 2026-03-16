using Api.Models;

namespace Api.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmail(string email);
        Task<User> CreateUser(User user);
        Task UpdateUser(User user);
    }
}
