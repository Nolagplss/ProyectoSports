namespace SportsCenterApi.Models.DTO
{
    public class PermissionsDTO
    {
      
        public int PermissionId { get; set; }
        public string Description { get; set; } = null!;

        public string Code { get; set; } = null!;
    }
}
