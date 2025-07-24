using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {

        Task<Role?> UpdateAsyncRole(Role role);

        Task<bool> ExistsAsync(int id);
        Task<Role?> GetByIdWithPermissionsAsync(int id);

        Task<IEnumerable<Permission>> GetPermissionsByIdsAsync(IEnumerable<int> permissionIds);

    }
}
