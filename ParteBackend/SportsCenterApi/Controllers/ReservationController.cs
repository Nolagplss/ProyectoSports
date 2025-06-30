using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Services;
using System.Security.Claims;

namespace SportsCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {

        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReservationsAsync()
        {
            return Ok(await _reservationService.GetAllAsync());

        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            return Ok(await _reservationService.GetByUserIdAsync(userId));
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDateReservationsAsync([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            return Ok(await _reservationService.GetByDateReservationAsync(startDate, endDate));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!DateOnly.TryParse(dto.ReservationDate, out var parsedDate))
                return BadRequest("The date format is invalid. Expected format: yyyy-MM-dd.");

            if (!TimeOnly.TryParse(dto.StartTime, out var parsedStart))
                return BadRequest("The start time format is invalid. Expected format: HH:mm.");

            if (!TimeOnly.TryParse(dto.EndTime, out var parsedEnd))
                return BadRequest("The end time format is invalid. Expected format: HH:mm.");

            if (parsedEnd <= parsedStart)
                return BadRequest("End time must be later than start time.");


            var reservation = new Reservation
            {
                UserId = dto.UserId,
                FacilityId = dto.FacilityId,
                ReservationDate = DateOnly.Parse(dto.ReservationDate),
                StartTime = TimeOnly.Parse(dto.StartTime),
                EndTime = TimeOnly.Parse(dto.EndTime),
                PaymentCompleted = dto.PaymentCompleted,
                NoShow = false
            };

            return Ok(await _reservationService.CreateAsync(reservation));
        }

       
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            //Check if the user is authorized to delete this reservation
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var userPermissions = User.FindAll("permission").Select(c => c.Value).ToList();

            if (userPermissions.Contains("CANCEL_OTHERS_UNLIMITED"))
            {
                //Can cancell any reservation without restrictions
                await _reservationService.DeleteAsync(id);
                return NoContent();
            }

            if(userPermissions.Contains("CANCEL_OWN_RESERVATIONS") && reservation.UserId == currentUserId)
            {
                //Can cancell their own reservation
                await _reservationService.DeleteAsync(id);
                return NoContent();
            }

            /*
            if (userPermissions.Contains("CANCEL_OTHERS_LIMITED"))
            {
             

                bool trueRestrictions = "";
                if (trueRestrictions)
                {
                    await _reservationService.DeleteAsync(id);
                    return NoContent();
                }
            }
            */


          
            return Forbid();
        }

    }
}
