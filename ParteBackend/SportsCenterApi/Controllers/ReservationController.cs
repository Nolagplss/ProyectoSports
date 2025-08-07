using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsCenterApi.Extensions;
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
        private readonly ItokenService _tokenService;
        private readonly ILogger<ReservationController> _logger;


        public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;

            _logger = logger;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationResponseDTO>>> GetAllReservationsAsync()
        {
            _logger.LogInformation("Fetching all reservations");

            var reservations = await _reservationService.GetAllAsync();
            
            var reservationDTO = reservations.Select(u => u.ToReservationResponseDTO()).ToList();

            _logger.LogInformation("Returned {Count} reservations", reservationDTO.Count);

            return Ok(reservationDTO);

        }



        [Authorize (Roles = "Facility Manager,Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReservation(int id, ReservationCreateDto dto)
        {

            try
            {
                _logger.LogInformation("Updating reservation with the reservation id");

                var updateReservation = await _reservationService.UpdateAsyncReservation(id, dto);

                if (updateReservation == null)
                {
                    _logger.LogWarning("Reservation with ID {ReservationId} not found for update", id);
                    return NotFound();
                }

                _logger.LogInformation("Reservation with ID {ReservationId} updated successfully", id);

                return Ok(updateReservation.ToReservationResponseDTO());
            }
             catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict updating reservation with ID {ReservationID}", id);

                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request updating reservation with ID {ReservationId}", id);

                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationResponseDTO>> GetById(int id)
        {
            var reserva = await _reservationService.GetByIdAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            return Ok(reserva);
        }



        [HttpGet("filter")]
        public async Task<IActionResult> FilterReservations(
            [FromQuery] int? userId, [FromQuery] string? facilityType, [FromQuery] string? facilityName, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            _logger.LogInformation("Filtering reservations with userId: {UserId}, facilityType: {FacilityType}, facilityName: {FacilityName}, startDate: {StartDate}, endDate: {EndDate}",
             userId, facilityType, facilityName, startDate, endDate);

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                _logger.LogWarning("Start date {StartDate} is after end date {EndDate}", startDate, endDate);
                return BadRequest("Start date cannot be after end date.");
            }

            var reservations = await _reservationService.FilterReservationsAsync(userId, facilityType, facilityName, startDate, endDate);

            _logger.LogInformation("Filter returned {Count} reservations", reservations.Count());


            return Ok(reservations);
        }

        [Authorize(Policy = "MakeReservations")]
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDto dto)
        {

            _logger.LogInformation("Creating reservation for userId {UserId}, facilityId {FacilityId}", dto.UserId, dto.FacilityId);


            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state");
                return BadRequest(ModelState);
            }

            if (!DateOnly.TryParse(dto.ReservationDate, out var parsedDate))
            {
                _logger.LogWarning("Invalid reservation date format: {ReservationDate}", dto.ReservationDate);
                return BadRequest("The date format is invalid. Expected format: yyyy-MM-dd.");
            }

            if (!TimeOnly.TryParse(dto.StartTime, out var parsedStart))
            {
                _logger.LogWarning("Invalid start time format: {StartTime}", dto.StartTime);
                return BadRequest("The start time format is invalid. Expected format: HH:mm.");
            }

            if (!TimeOnly.TryParse(dto.EndTime, out var parsedEnd))
            {
                _logger.LogWarning("Invalid end time format: {EndTime}", dto.EndTime);
                return BadRequest("The end time format is invalid. Expected format: HH:mm.");
            }

            if (parsedEnd <= parsedStart)
            {
                _logger.LogWarning("End time {EndTime} is not later than start time {StartTime}", dto.EndTime, dto.StartTime);
                return BadRequest("End time must be later than start time.");
            }

            //Validation for 
            var userPermissions = User.FindAll("permission").Select(p => p.Value).ToList();
            var isAdmin = userPermissions.Contains("RESERVE_UNLIMITED");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                _logger.LogWarning("User unauthorized: missing NameIdentifier claim");
                return Unauthorized();
            }

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
                    _logger.LogWarning("User {CurrentUserId} attempted to create reservation for user {UserId} without permission", currentUserId, dto.UserId);

                    return Forbid("You can only make reservations for yourself.");
                }
                //Create reservation with validation
                var createdReservation = await _reservationService.CreateReservationWithValidationAsync(reservation, isAdmin);

                _logger.LogInformation("Reservation created with ID {ReservationId}", createdReservation.ReservationId);


                return Ok(createdReservation);

            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to create reservation due to invalid operation");

                return BadRequest(ex.Message);
            }

           
        }

       
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            _logger.LogInformation("Deleting reservation with ID {ReservationId}", id);

            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
            {
                _logger.LogWarning("Reservation with ID {ReservationId} not found", id);

                return NotFound();
            }
            //Check if the user is authorized to delete this reservation
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                _logger.LogWarning("User unauthorized: missing NameIdentifier claim");
                return Unauthorized();
            }

            int currentUserId = int.Parse(userIdClaim);

            var userPermissions = User.FindAll("permission").Select(c => c.Value).ToList();

            if (userPermissions.Contains("CANCEL_OTHERS_UNLIMITED"))
            {
                _logger.LogInformation("User {UserId} deleting any reservation without restriction", currentUserId);

                //Can cancell any reservation without restrictions
                await _reservationService.DeleteAsync(id);
                return NoContent();
            }

            if(userPermissions.Contains("CANCEL_OWN_RESERVATIONS") && reservation.UserId == currentUserId)
            {
                _logger.LogInformation("User {UserId} deleting own reservation with ID {ReservationId}", currentUserId, id);


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
                    _logger.LogInformation("User {UserId} deleting future reservation {ReservationId} with limited permissions", currentUserId, id);

                    await _reservationService.DeleteAsync(id);
                    return NoContent();
                }


            }



            _logger.LogWarning("User {UserId} forbidden to delete reservation {ReservationId}", currentUserId, id);

            return Forbid();
        }


        [Authorize]
        [HttpPost("reservations/{id}/noshow")]
        public async Task<IActionResult> MarkNoShow(int id)
        {
            _logger.LogInformation("Marking reservation {ReservationId} as NoShow", id);

            try
            {
                _logger.LogInformation("Reservation {ReservationId} marked as NoShow", id);

                await _reservationService.MarkNoShowAsync(id);

                return Ok(new { Message = "Reservation changed to NoShow" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking reservation {ReservationId} as NoShow", id);

                return BadRequest(new { Error = ex.Message });
            }


        }

    }
}
