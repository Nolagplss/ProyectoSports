using Microsoft.AspNetCore.Mvc;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Services;

namespace SportsCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ItokenService _tokenService;

        public AuthController(IUserService userService, ItokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        /*
         * --------------------Try with--------------------
         * luis.garcia@email.com 1234 (Administrator), 
         * pedro.martinez@email.com 12345 (Facility Manager), 
         * juan.perez@email.com clave123(Member)
        */
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }
    }
}
