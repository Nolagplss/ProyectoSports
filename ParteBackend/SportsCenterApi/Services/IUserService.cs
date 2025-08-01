﻿using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Services
{
    public interface IUserService : IGenericService<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> RegisterUserAsync(User user);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<User?> UpdateAsyncUser(int id, UserDTO userDTO);
    }
}
