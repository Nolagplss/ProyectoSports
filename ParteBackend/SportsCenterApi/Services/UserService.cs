using SportsCenterApi.Models;
using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Repositories;
using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Services
{
    public class UserService : GenericService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }


        //User registration with validation
        public async Task<User> RegisterUserAsync(User user)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists.");

            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FirstName))
                throw new ArgumentException("Email and name are required.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            return await _userRepository.AddAsync(user);
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        //Autentication
        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            return await _userRepository.AuthenticateAsync(email, password);

        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            //Get the current user
            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
            {
                throw new ArgumentException("User not found");
            }
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                throw new UnauthorizedAccessException("Current password is incorrect");
            }


            //Assign the new password to the current user;
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user);

            return true;

        }
        public async Task<User?> UpdateAsyncUser(int id, UserDTO userDTO)
        {
            //Get the existing user
            var existingUser = await _repository.GetByIdAsync(id);
            if (existingUser == null)
                return null;

            //Validate
            if (string.IsNullOrWhiteSpace(userDTO.FirstName))
                throw new ArgumentException("The name is required");

            //Update the basic properties
            existingUser.FirstName = userDTO.FirstName;
            existingUser.LastName = userDTO.LastName;
            existingUser.Email = userDTO.Email;
            existingUser.Phone = userDTO.Phone;
            existingUser.RoleId = userDTO.RoleId;

            if (!string.IsNullOrWhiteSpace(userDTO.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            }

            //Update on the repository
            return await _repository.UpdateAsync(existingUser);
        }



    }
}
