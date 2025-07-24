using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsCenterApi.Extensions;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Services;

namespace SportsCenterApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FacilitiesController : ControllerBase
    {

        private readonly IFacilityService _facilityService;


        public FacilitiesController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityDTO>>> GetAllFacilities()
        {

            var facilities = await _facilityService.GetAllAsync();
            var dtoFacilities = facilities.Select(f => f.ToFacilityDto());
            return Ok(dtoFacilities);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityDTO>> GetFacilityById(int id)
        {
            var facility = await _facilityService.GetByIdAsync(id);
            if (facility == null)
                return NotFound();

            var dto = facility.ToFacilityDto();
            return Ok(dto);
        }
            
        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpPost]
        public async Task<ActionResult<Facility>> CreateFacility([FromBody] FacilityDTO dto)
        {
            try
            {
                var facility = dto.ToFacilityEntity();

                var createdFacility = await _facilityService.CreateAsync(facility);

                //To return only a dto
                var createdDto = createdFacility.ToFacilityDto();

                return CreatedAtAction(nameof(GetFacilityById), new { id = createdDto.FacilityId }, createdDto);
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

        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFacility(int id, FacilityDTO dto)
        {

            var updateFacility = await _facilityService.UpdateAsync(id, dto.ToFacilityEntity());

            if (updateFacility == null)
                return NotFound();

            return Ok(updateFacility);
        }

        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacility(int id, FacilityDTO dto)
        {

            var facility = await _facilityService.GetByIdAsync(id);

            if(facility == null)
                return NotFound();

            await _facilityService.DeleteAsync(id);

            return NoContent();

        }


        //Try using 2025-06-17 from 12:00 to 13:00 to see that one facility has a reservation and does not appear in the results
        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<FacilityDTO>>> FilterFacilities(
            [FromQuery] string? type,
            [FromQuery] DateOnly? date,
            [FromQuery] TimeOnly? startTime,
            [FromQuery] TimeOnly? endTime)
        {
            var facilitiesDTO = await _facilityService.FilterFacilitiesAsync(type, date, startTime, endTime);
            return Ok(facilitiesDTO);
        }


    }
}
