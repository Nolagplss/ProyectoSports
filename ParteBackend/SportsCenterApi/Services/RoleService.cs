using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Repositories;

namespace SportsCenterApi.Services
{
    public class RoleService : GenericService<Role>, IRoleService 
    {
        private readonly IRoleRepository _repository;

        public RoleService(IRoleRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public override async Task<Role?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdWithPermissionsAsync(id);

        }

        public async Task<Role?> UpdateAsyncRole(int id, RoleDTO roleDto)
        {
            //Get the existing role
            var existingRole = await _repository.GetByIdWithPermissionsAsync(id);
            if (existingRole == null)
                return null;

            //Validate
            if (string.IsNullOrWhiteSpace(roleDto.RoleName))
                throw new ArgumentException("The role name is required");

            //Update the basic properties
            existingRole.RoleName = roleDto.RoleName;

            //Permissions
            if (roleDto.Permissions != null && roleDto.Permissions.Any())
            {
                //Remove the existed permissions
                existingRole.Permissions.Clear();

                //Get the valid permissions
                var permissionIds = roleDto.Permissions
                    .Where(p => p.PermissionId > 0)
                    .Select(p => p.PermissionId)
                    .Distinct()
                    .ToList();

                if (permissionIds.Any())
                {
                    //Get the permissions
                    var permissions = await _repository.GetPermissionsByIdsAsync(permissionIds);

                    //Atacch the permissions to the role
                    foreach (var permission in permissions)
                    {
                        existingRole.Permissions.Add(permission);
                    }
                }
            }

            //Update on the repository
            return await _repository.UpdateAsync(existingRole);
        }


    }
}
