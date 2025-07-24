using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Services
{
    public interface IRoleService : IGenericService<Role>
    {

        Task<Role?> UpdateAsyncRole(int id, RoleDTO roleDto);

    }
}
