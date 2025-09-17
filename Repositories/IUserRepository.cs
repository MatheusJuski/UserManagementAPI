using UserManagementAPI.Models;

namespace UserManagementAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);

        Task SaveChangesAsync();
        
    }
}