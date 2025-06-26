using SportsCenterApi.Models;
using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Repositories;

namespace SportsCenterApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

     
        public async Task<bool> RegisterUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                throw new Exception("User with this email already exists.");

            await _userRepository.AddAsync(user);
            return true;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }
    }
}
