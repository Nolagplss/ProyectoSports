using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Extensions
{
    public static class UserExtensions
    {

        public static UserDTO ToUserDTO(this User user)
        {
            return new UserDTO()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Phone = user.Phone,
                RoleId = user.RoleId

            };
        }

        public static User ToUserEntity(this UserDTO dto)
        {
            return new User
            {
                UserId = dto.UserId == 0 ? 0 : dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone,
                RoleId = dto.RoleId
            };
        }

    }
}
