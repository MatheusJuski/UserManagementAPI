using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<User?> ValidateUserCredentialsAsync(string username, string password);
        Task<bool> DeleteUserAsync(int id);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        // Métodos de Refresh Token
        Task SetRefreshTokenAsync(User user);
        string GenerateRefreshToken();
    }
}
