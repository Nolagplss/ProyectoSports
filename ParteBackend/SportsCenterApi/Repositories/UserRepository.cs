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
            var user = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            bool passwordMatches = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!passwordMatches)
                return null;

            return user;    
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(r => r.UserId == id);
        }

        //Update the role
        public async Task<User?> UpdateAsyncUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(user.UserId))
                    return null;
                throw;
            }
        }
     
    }
}
