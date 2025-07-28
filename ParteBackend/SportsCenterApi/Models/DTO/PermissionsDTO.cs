using System.ComponentModel.DataAnnotations;

namespace SportsCenterApi.Models.DTO
{
    public class PermissionsDTO
    {
        [Required]
        public int PermissionId { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;
    }
}
