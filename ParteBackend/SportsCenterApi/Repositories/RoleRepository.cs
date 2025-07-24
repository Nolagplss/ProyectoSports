using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using System.Data;

namespace SportsCenterApi.Repositories
{
    public class RoleRepository : GenericRespository<Role>, IRoleRepository
    {
        public RoleRepository(SportsCenterContext context) : base(context)
        {

        }
        //Take all the roles
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Roles.AnyAsync(r => r.RoleId == id);
        }

        //Update the role
        public async Task<Role?> UpdateAsyncRole(Role role)
        {
            _context.Entry(role).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return role;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(role.RoleId))
                    return null;
                throw;
            }
        }
        public async Task<Role?> GetByIdWithPermissionsAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.RoleId == id);
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByIdsAsync(IEnumerable<int> permissionIds)
        {
            return await _context.Permissions
                .Where(p => permissionIds.Contains(p.PermissionId))
                .ToListAsync();
        }
    }
}
