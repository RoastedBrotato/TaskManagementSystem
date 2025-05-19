using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Interfaces;

namespace TaskManagementSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;

        public UserService(IUserRepository userRepository, IPasswordHashService passwordHashService)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user != null && _passwordHashService.VerifyPassword(user.Password, password))
            {
                return user;
            }

            return null;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Hash password before saving
            user.Password = _passwordHashService.HashPassword(user.Password);
            return await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            // If updating password, hash it
            if (!string.IsNullOrEmpty(user.Password))
            {
                var existingUser = await _userRepository.GetByIdAsync(user.Id);
                if (existingUser.Password != user.Password)
                {
                    user.Password = _passwordHashService.HashPassword(user.Password);
                }
            }
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
    }
}