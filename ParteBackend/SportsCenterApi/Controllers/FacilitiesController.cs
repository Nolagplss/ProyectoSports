using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
