﻿using SportsCenterApi.Models;
using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Repositories;

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
            if(user.Password != currentPassword)
            {
                throw new UnauthorizedAccessException("Current password is incorrect");
            }

            //Assign the new password to the current user;
            user.Password = newPassword;

            await _userRepository.UpdateAsync(user);

            return true;

        }



    }
}
