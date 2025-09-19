using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repository.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _repository.GetUserByUsernameAsync(username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _repository.AddUserAsync(user);
            await _repository.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existing = await _repository.GetUserByIdAsync(id);
            if (existing == null) return null;

            existing.Username = user.Username;
            existing.Email = user.Email;
            existing.PasswordHash = user.PasswordHash;
            existing.Role = user.Role;

            _repository.UpdateUser(existing);
            await _repository.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return false;

            _repository.DeleteUser(user);
            await _repository.SaveChangesAsync();
            return true;
        }
        
    }
}
