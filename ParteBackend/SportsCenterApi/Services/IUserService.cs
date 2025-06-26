using SportsCenterApi.Models;

namespace SportsCenterApi.Services
{
    public interface IUserService
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> RegisterUserAsync(User user);
    }
}
