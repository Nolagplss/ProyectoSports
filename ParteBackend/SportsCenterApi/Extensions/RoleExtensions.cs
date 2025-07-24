using SportsCenterApi.Models.DTO;
using SportsCenterApi.Models;

namespace SportsCenterApi.Extensions
{
    public static class RoleExtensions
    {

        public static RoleDTO ToRoleDTO(this Role role)
        {
            return new RoleDTO
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Permissions = role.Permissions.Select(p => new PermissionsDTO
                {

                    PermissionId = p.PermissionId,
                    Description = p.Description,
                    Code = p.Code

                }).ToList()

            };
        }

        public static Role ToRoleEntity(this RoleDTO dto)
        {
            return new Role
            {
                RoleId = 0,
                RoleName = dto.RoleName,
                Permissions = new List<Permission>()
            };
        }


    }
}
