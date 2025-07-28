using System.ComponentModel.DataAnnotations;

namespace SportsCenterApi.Models.DTO
{
    public class RoleDTO
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = null!;
        public ICollection<PermissionsDTO> Permissions { get; set; } = new List<PermissionsDTO>();

    }
}
