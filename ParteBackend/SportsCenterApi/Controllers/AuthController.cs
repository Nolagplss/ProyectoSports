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
        private readonly ILogger<AuthController> _logger;
        public AuthController(IUserService userService, ItokenService tokenService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
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
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", dto.Email);
                return Unauthorized("Invalid credentials");
            }

            var token = _tokenService.CreateToken(user);

            _logger.LogInformation("User {Email} logged in successfully", dto.Email);

            return Ok(new { token });
        }
    }
}
