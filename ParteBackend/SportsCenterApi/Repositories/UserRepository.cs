using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public class UserRepository : GenericRespository<User>, IUserRepository
    {
      
        public UserRepository(SportsCenterContext context) : base(context)
        {
           
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            return await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

    }
}
