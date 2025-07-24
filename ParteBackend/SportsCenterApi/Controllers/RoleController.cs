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


        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAllRoles()
        {

            var roles = await _roleService.GetAllAsync();

            if(roles == null)
                return NotFound();

            var dtoRoles = roles.Select(f => f.ToRoleDTO());


            return Ok(dtoRoles);

        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
                return NotFound();

            var dto = role.ToRoleDTO();
            return Ok(dto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleDTO dto)
        {
            try
            {

                var role = dto.ToRoleEntity();

                var createdRole = await _roleService.CreateAsync(role);

                //To return only a dto
                var createdDto = createdRole.ToRoleDTO();

                return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.RoleId }, createdDto);
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
        public async Task<ActionResult> UpdateRole(int id, RoleDTO dto)
        {

            try
            {
                
                var updatedRole = await _roleService.UpdateAsyncRole(id, dto);
                if (updatedRole == null)
                    return NotFound();

                return Ok(updatedRole.ToRoleDTO());
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id, RoleDTO dto)
        {

            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
                return NotFound();

            await _roleService.DeleteAsync(id);

            return NoContent();

        }



    }
}
