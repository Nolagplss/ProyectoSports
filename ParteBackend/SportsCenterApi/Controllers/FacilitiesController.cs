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

        private readonly ILogger<FacilitiesController> _logger;

        public FacilitiesController(IFacilityService facilityService, ILogger<FacilitiesController> logger)
        {
            _facilityService = facilityService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityDTO>>> GetAllFacilities()
        {
            _logger.LogInformation("Getting all facilities");

            var facilities = await _facilityService.GetAllAsync();
            var dtoFacilities = facilities.Select(f => f.ToFacilityDto());

            _logger.LogInformation("Returned {Count} facilities", dtoFacilities.Count());


            return Ok(dtoFacilities);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityDTO>> GetFacilityById(int id)
        {
            _logger.LogInformation("Getting facility with ID {FacilityId}", id);


            var facility = await _facilityService.GetByIdAsync(id);
            if (facility == null)
            {
                _logger.LogWarning("Facility with ID {FacilityId} not found", id);
                return NotFound();
            }

            var dto = facility.ToFacilityDto();

            _logger.LogInformation("Facility with ID {FacilityId} found", id);

            return Ok(dto);
        }
            
        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpPost]
        public async Task<ActionResult<Facility>> CreateFacility([FromBody] FacilityDTO dto)
        {
            _logger.LogInformation("Creating a new facility");
            try
            {
                var facility = dto.ToFacilityEntity();

                var createdFacility = await _facilityService.CreateAsync(facility);

                //To return only a dto
                var createdDto = createdFacility.ToFacilityDto();

                _logger.LogInformation("Facility created with ID {FacilityId}", createdDto.FacilityId);


                return CreatedAtAction(nameof(GetFacilityById), new { id = createdDto.FacilityId }, createdDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict creating facility");

                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request creating facility");

                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFacility(int id, FacilityDTO dto)
        {
            _logger.LogInformation("Updating facility with ID {FacilityId}", id);

            var updateFacility = await _facilityService.UpdateAsync(id, dto.ToFacilityEntity());

            if (updateFacility == null)
            {
                _logger.LogWarning("Facility with ID {FacilityId} not found for update", id);
                return NotFound();
            }
            _logger.LogInformation("Facility with ID {FacilityId} updated successfully", id);


            return Ok(updateFacility);
        }

        [Authorize(Roles = "Facility Manager,Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacility(int id, FacilityDTO dto)
        {
            _logger.LogInformation("Deleting facility with ID {FacilityId}", id);

            var facility = await _facilityService.GetByIdAsync(id);

            if (facility == null)
            {
                _logger.LogWarning("Facility with ID {FacilityId} not found for deletion", id);
                return NotFound();
            }

            await _facilityService.DeleteAsync(id);
            _logger.LogInformation("Facility with ID {FacilityId} deleted", id);



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
            _logger.LogInformation("Filtering facilities with Type: {Type}, Date: {Date}, StartTime: {StartTime}, EndTime: {EndTime}",
               type, date, startTime, endTime);

            var facilitiesDTO = await _facilityService.FilterFacilitiesAsync(type, date, startTime, endTime);

            _logger.LogInformation("Filter returned {Count} facilities", facilitiesDTO.Count());

            return Ok(facilitiesDTO);
        }


    }
}
