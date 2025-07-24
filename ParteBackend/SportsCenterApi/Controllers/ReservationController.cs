using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Services;
using System.Security.Claims;

namespace SportsCenterApi.Controllers
{                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    //CODE BY SAMUEL RADU DRAGOMIR
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

        [HttpGet("filter")]
        public async Task<IActionResult> FilterReservations(
            [FromQuery] int? userId, [FromQuery] string? facilityType, [FromQuery] string? facilityName, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                return BadRequest("Start date cannot be after end date.");

            var reservations = await _reservationService.FilterReservationsAsync(userId, facilityType, facilityName, startDate, endDate);
            return Ok(reservations);
        }

        [Authorize(Policy = "MakeReservations")]
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

            //Validation for 
            var userPermissions = User.FindAll("permission").Select(p => p.Value).ToList();
            var isAdmin = userPermissions.Contains("RESERVE_UNLIMITED");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);


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

            try
            {
                //Only can make reservations by their self
                if (!isAdmin && dto.UserId != currentUserId)
                {
                    return Forbid("You can only make reservations for yourself.");
                }
                //Create reservation with validation
                var createdReservation = await _reservationService.CreateReservationWithValidationAsync(reservation, isAdmin);

                return Ok(createdReservation);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

           
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

                //Penalize
                await _reservationService.PenalizeMemberIfLateCancellationAsync(reservation);


                return NoContent();
            }

            //Cancell other reservations
            if (userPermissions.Contains("CANCEL_OTHERS_LIMITED") && reservation.UserId != currentUserId)
            {
                //Time Now
                var timeNow = DateTime.Now;

                //Time of reservation
                var reservationDateTime = reservation.ReservationDate.ToDateTime(reservation.StartTime);

                //Check if the reservation is in the future
                if (timeNow < reservationDateTime)
                {
                    await _reservationService.DeleteAsync(id);
                    return NoContent();
                }


            }
            


          
            return Forbid();
        }


        [Authorize]
        [HttpPost("reservations/{id}/noshow")]
        public async Task<IActionResult> MarkNoShow(int id)
        {

            try
            {
                await _reservationService.MarkNoShowAsync(id);

                return Ok(new { Message = "Reservation changed to NoShow" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }


        }

    }
}
