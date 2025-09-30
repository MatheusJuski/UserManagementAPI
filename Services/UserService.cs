using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;
using System.Security.Cryptography;

namespace UserManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<User>();
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
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
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
        public async Task<User?> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _repository.GetUserByUsernameAsync(username);
            if (user == null) return null;

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return user;
}


        public async Task SetRefreshTokenAsync(User user)
        {
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // expira em 7 dias
            await _repository.SaveChangesAsync();
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var users = await _repository.GetAllUsersAsync();
            return users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }

        
    }
}
