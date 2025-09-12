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
        private readonly ILogger<UsersController> _logger;

        //Injecting the IUserService to handle user-related operations
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            _logger.LogInformation("Getting all users");

            var users = await _userService.GetAllAsync();
            var userDtos = users.Select(u => u.ToUserResponseDTO()).ToList();

            _logger.LogInformation("Returning {Count} users", userDtos.Count);


            return Ok(userDtos);
        }
        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDTO>> GetUser(int id)
        {
            _logger.LogInformation("Getting user with ID {UserId}", id);

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound();
            }

            var userDto = user.ToUserResponseDTO();

            return Ok(userDto);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> CreateUser(UserCreateDto userDto)
        {

            _logger.LogInformation("Creating user with email {Email}", userDto.Email);


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
                var responseDTO = createdUser.ToUserResponseDTO();

                _logger.LogInformation("User created with ID {UserId}", createdUser.UserId);


                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, responseDTO);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict creating user with email {Email}", userDto.Email);

                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request creating user with email {Email}", userDto.Email);

                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDTO>> UpdateUser(int id, UserDTO userDTO)
        {
            _logger.LogInformation("Updating user with ID {UserId}", id);

            if (id != userDTO.UserId)
            {
                _logger.LogWarning("User ID in URL does not match ID in body");
                return BadRequest();
            }

            var updatedUser = await _userService.UpdateAsyncUser(id, userDTO);
            if (updatedUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", id);
                return NotFound();
            }

            var responseDto = updatedUser.ToUserResponseDTO();

            _logger.LogInformation("User with ID {UserId} updated", id);


            return Ok(responseDto);
        }
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)  
        {
            _logger.LogInformation("Deleting user with ID {UserId}", id);


            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return NotFound();
            }
            _logger.LogInformation("User with ID {UserId} deleted", id);

            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
          
            ChangePasswordDTO dto)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                _logger.LogWarning("Unauthorized attempt to change password");
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            _logger.LogInformation("User with ID {UserId} changing own password", userId);

            //Now we use the userId to change the password
            var result = await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

            if (!result)
            {
                _logger.LogWarning("User with ID {UserId} provided incorrect current password", userId);
                return BadRequest("Current password is incorrect.");
            }
            _logger.LogInformation("User with ID {UserId} changed password successfully", userId);

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
            {
                _logger.LogWarning("Unauthorized attempt to change others' password");
                return Unauthorized();
            }

            //Parse the id to int
            int userId = int.Parse(userIdClaim);

            //Find the permissions
            var userPermissions = User.FindAll("permission").Select(c => c.Value).ToList();

            if (id != userId && !userPermissions.Contains("CHANGE_OTHERS_PASSWORD"))
            {
                _logger.LogWarning("User with ID {UserId} attempted to change password for user {TargetId} without permission", userId, id);

                return Forbid();

            }

            //Change the password
            var success = await _userService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);

            if (!success)
            {
                _logger.LogWarning("Failed to change password for user {TargetId}", id);

                return NotFound(new { message = "User not found" });
            }
            _logger.LogInformation("Password for user {TargetId} changed successfully by user {UserId}", id, userId);

            return Ok("Password changed successfully.");


        }
    }
}
