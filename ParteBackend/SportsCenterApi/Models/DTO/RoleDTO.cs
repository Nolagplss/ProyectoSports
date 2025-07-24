namespace SportsCenterApi.Models.DTO
{
    public class RoleDTO
    {

        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public ICollection<PermissionsDTO> Permissions { get; set; } = new List<PermissionsDTO>();

    }
}
