using System.ComponentModel.DataAnnotations;

namespace SportsCenterApi.Models.DTO
{
    public class UserResponseDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
