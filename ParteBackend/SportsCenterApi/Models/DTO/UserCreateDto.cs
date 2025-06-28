namespace SportsCenterApi.Models.DTO
{
    //DTO for the user without the member
    public class UserCreateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Phone { get; set; }
        public int RoleId { get; set; }
    }
}
