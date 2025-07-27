using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SportsCenterApi.Models.DTO
{
    public class UserDTO
    {

        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Password { get; set; }

        public string? Phone { get; set; }

        public int RoleId { get; set; }

    }
}
