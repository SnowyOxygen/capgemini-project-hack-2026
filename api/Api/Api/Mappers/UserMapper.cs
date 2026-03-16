using Api.DTOs;
using Api.Models;

namespace Api.Mappers
{
    public static class UserMapper
    {
        public static User ToUser(this SignUpRequest request, string hashedPassword, int roleId)
        {
            return new User
            {
                Email = request.Email,
                Password = hashedPassword,
                RoleId = roleId
            };
        }
    }
}
