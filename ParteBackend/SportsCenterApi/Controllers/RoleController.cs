using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsCenterApi.Extensions;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Services;

namespace SportsCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAllRoles()
        {
            _logger.LogInformation("Fetching all roles");

            var roles = await _roleService.GetAllAsync();

            if (roles == null)
            {
                _logger.LogWarning("No roles found");
                return NotFound();
            }

            var dtoRoles = roles.Select(f => f.ToRoleDTO());

            _logger.LogInformation("Returning {Count} roles", dtoRoles.Count());


            return Ok(dtoRoles);

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(int id)
        {
            _logger.LogInformation("Fetching role with ID {RoleId}", id);

            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found", id);
                return NotFound();
            }

            var dto = role.ToRoleDTO();

            _logger.LogInformation("Returning role with ID {RoleId}", id);


            return Ok(dto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleDTO dto)
        {
            _logger.LogInformation("Creating new role");

            try
            {

                var role = dto.ToRoleEntity();

                var createdRole = await _roleService.CreateAsync(role);

                //To return only a dto
                var createdDto = createdRole.ToRoleDTO();

                _logger.LogInformation("Role created with ID {RoleId}", createdRole.RoleId);


                return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.RoleId }, createdDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict creating role");

                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request creating role");

                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(int id, RoleDTO dto)
        {
            _logger.LogInformation("Updating role with ID {RoleId}", id);

            try
            {
                
                var updatedRole = await _roleService.UpdateAsyncRole(id, dto);
                if (updatedRole == null)
                {
                    _logger.LogWarning("Role with ID {RoleId} not found for update", id);
                    return NotFound();
                }
                _logger.LogInformation("Role with ID {RoleId} updated", id);


                return Ok(updatedRole.ToRoleDTO());
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict updating role with ID {RoleId}", id);

                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request updating role with ID {RoleId}", id);

                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id, RoleDTO dto)
        {
            _logger.LogInformation("Deleting role with ID {RoleId}", id);

            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found for deletion", id);
                return NotFound();
            }
            await _roleService.DeleteAsync(id);

            _logger.LogInformation("Role with ID {RoleId} deleted", id);


            return NoContent();

        }



    }
}
