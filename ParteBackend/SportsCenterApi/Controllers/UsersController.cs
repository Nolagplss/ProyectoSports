using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsCenterApi.Models;
using SportsCenterApi.Services;
using SportsCenterApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SportsCenterApi.Extensions;

namespace SportsCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        //Injecting the IUserService to handle user-related operations
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(UserCreateDto userDto)
        {
            try
            {
                var user = new User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Password = userDto.Password,
                    Phone = userDto.Phone,
                    RoleId = userDto.RoleId
                };

                var createdUser = await _userService.RegisterUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.UserId)
                return BadRequest();

            var updatedUser = await _userService.UpdateAsyncUser(id, userDTO);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)  
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
          
            ChangePasswordDTO dto)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            //Now we use the userId to change the password
            var result = await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

            if (!result)
                return BadRequest("Current password is incorrect.");

            return Ok("Password changed successfully.");
        }

        [Authorize]
        [HttpPost("{id}/change-Others-password")]
        public async Task<IActionResult> ChangeOthersPassword(
            int id,
            [FromBody] ChangePasswordDTO dto)
        {

            //Get the currentUser
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            //Parse the id to int
            int userId = int.Parse(userIdClaim);

            //Find the permissions
            var userPermissions = User.FindAll("permission").Select(c => c.Value).ToList();

            if (id != userId && !userPermissions.Contains("CHANGE_OTHERS_PASSWORD"))
            {
                return Forbid("You do not have permission to change other users passwords.");
            }
            
            //Change the password
            var success = await _userService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);

            if (!success)
            {
                return Forbid("User not fount");
            }

            return Ok("Password changed successfully.");


        }
    }
}
