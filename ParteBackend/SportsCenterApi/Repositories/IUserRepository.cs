using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> AuthenticateAsync(string email, string password);


        Task<User?> UpdateAsyncUser(User user);

    

    }
}
