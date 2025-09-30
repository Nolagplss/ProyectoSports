using Microsoft.AspNetCore.Razor.TagHelpers;
using SportsCenterApi.Extensions;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Repositories;
using System.Security.Claims;

namespace SportsCenterApi.Services
{
    public class ReservationService : GenericService<Reservation>, IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        private readonly IFacilitiesRepository _facilitiesRepository;

        public ReservationService(IReservationRepository reservationRepository, IFacilitiesRepository facilitiesRepository) : base(reservationRepository)
        {
            _reservationRepository = reservationRepository;
            _facilitiesRepository = facilitiesRepository;
        }

        public async Task<IEnumerable<ReservationWithFacilityDTO>> FilterReservationsAsync(int? userId, string? facilityType, string? facilityName, DateOnly? startDate, DateOnly? endDate)
        {
            return await _reservationRepository.FilterReservationsAsync(userId,facilityType, facilityName, startDate, endDate);
        }

        public async Task<Reservation> CreateReservationWithValidationAsync(Reservation reservation, bool isAdmin)
        {
           

            if (!isAdmin)
            {
                //Check if there is active the reservation
                var hasActive = await _reservationRepository.HasActiveReservationAsync(
                    reservation.UserId,
                    reservation.FacilityId,
                    DateOnly.FromDateTime(DateTime.Today)
                );


                if (hasActive)
                    throw new InvalidOperationException("You already have an active reservation for this facility.");

                //Check the valid range of reservations
                var today = DateOnly.FromDateTime(DateTime.Today);
                if (reservation.ReservationDate < today || reservation.ReservationDate > today.AddDays(15))
                    throw new InvalidOperationException("You can only make reservations within the next 15 days.");

                //Check the duration by installation
                var facility = await _reservationRepository.GetFacilityByIdAsync(reservation.FacilityId);
                if (facility == null)
                    throw new InvalidOperationException("Installation not found.");


                var duration = reservation.EndTime.ToTimeSpan() - reservation.StartTime.ToTimeSpan();

                if (duration.TotalHours < facility.MinReservationHours || duration.TotalHours > facility.MaxReservationHours)
                {
                    throw new InvalidOperationException("The reservation duration does not comply with the limits allowed by the facility.");
                }


                var userObj = await _reservationRepository.GetUserByIdAsync(reservation.UserId);
                if (userObj == null)
                    throw new InvalidOperationException("User not found.");

                if (userObj.Role.RoleName == "Member")
                {
                    if (userObj.Member?.Penalized == true &&
                        userObj.Member.PenaltyEndDate.HasValue &&
                        userObj.Member.PenaltyEndDate.Value.ToDateTime(new TimeOnly(0, 0)) > DateTime.Now)
                    {
                        throw new InvalidOperationException("The user is penalized and cannot make reservations.");
                    }
                }



            }

            return await _reservationRepository.AddAsync(reservation);
        }


        public async Task PenalizeMemberIfLateCancellationAsync(Reservation reservation)
        {
            //Take the user with that reservation
            var user = await _reservationRepository.GetUserByIdAsync(reservation.UserId);

            //Return if isn't a member
            if (user?.Role?.RoleName != "Member" || user.Member == null)
                return;

            //Time now
            var nowTimeOnly = TimeOnly.FromDateTime(DateTime.Now);
            // Reservation start time
            var dateReservationStartTime = reservation.StartTime;

            var dateTimeNow = DateTime.Now;

            if(nowTimeOnly.AddHours(1) > dateReservationStartTime)
            {
                //Penalize the member
                user.Member.Penalized = true;

                user.Member.PenaltyEndDate = DateOnly.FromDateTime(dateTimeNow);

                await _reservationRepository.SaveChangesAsync();

            }


            

        }


        public async Task MarkNoShowAsync(int reservationId)
        {

            //Get the reservation
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
                throw new Exception("Reservation not found");

            //True
            reservation.NoShow = true;

            //Update that reservation
            await _reservationRepository.UpdateAsync(reservation);


        }

        public async Task<Reservation?> UpdateAsyncReservation(int id, ReservationCreateDto dto)
        {
            //Get the existing reservation
            var existingReservation = await _reservationRepository.GetByIdAsync(id);

            if(existingReservation == null)
            {
                return null;
            }

            //validate
            if (dto.UserId <= 0)
                throw new ArgumentException("UserId is required and must be greater than 0");

            if (dto.FacilityId <= 0)
                throw new ArgumentException("FacilityId is required and must be greater than 0");

            if (string.IsNullOrWhiteSpace(dto.ReservationDate) ||
                !DateTime.TryParse(dto.ReservationDate, out var parsedDate))
                throw new ArgumentException("Invalid or missing ReservationDate");

            if (string.IsNullOrWhiteSpace(dto.StartTime) ||
                !TimeSpan.TryParse(dto.StartTime, out var parsedStartTime))
                throw new ArgumentException("Invalid or missing StartTime");

            if (string.IsNullOrWhiteSpace(dto.EndTime) ||
                !TimeSpan.TryParse(dto.EndTime, out var parsedEndTime))
                throw new ArgumentException("Invalid or missing EndTime");

            if (parsedEndTime <= parsedStartTime)
                throw new ArgumentException("EndTime must be after StartTime");

            //Update properties
            existingReservation.UserId = dto.UserId;
            existingReservation.FacilityId = dto.FacilityId;
            existingReservation.ReservationDate = DateOnly.FromDateTime(parsedDate);
            existingReservation.StartTime = TimeOnly.FromTimeSpan(parsedStartTime); ;
            existingReservation.EndTime = TimeOnly.FromTimeSpan(parsedEndTime);
            existingReservation.PaymentCompleted = dto.PaymentCompleted;

            return await _reservationRepository.UpdateAsync(existingReservation);

        }
        //Get all with the facilities
        public async Task<IEnumerable<Reservation>> GetAllReservationsWithFacilitiesAsync()
        {
            
            return await _reservationRepository.GetAllWithFacilitiesAsync();
        }
        //Get all with the facilities returning the dto.
        public async Task<IEnumerable<ReservationWithFacilityDTO>> GetAllReservationsWithFacilitiesDTOAsync()
        {
            var reservations = await _reservationRepository.GetAllWithFacilitiesAsync();
            return reservations.Select(r => r.ToReservationWithFacilityDTO());
        }

        public async Task<List<AvailableSlotsDTO>> GetAvailableSlotsAsync(int facilityId, DateOnly date)
        {
            Console.WriteLine($"[DEBUG] GetAvailableSlots called - FacilityId: {facilityId}, Date: {date}");

            //Get facility WITH SCHEDULES
            var facility = await _reservationRepository.GetFacilityByIdAsync(facilityId);
            if (facility == null)
            {
                Console.WriteLine("[DEBUG] Facility not found!");
                throw new KeyNotFoundException("Facility not found");
            }

            Console.WriteLine($"[DEBUG] Facility found: {facility.Name}, Type: {facility.Type}");
            Console.WriteLine($"[DEBUG] FacilitySchedules count: {facility.FacilitySchedules?.Count ?? 0}");

            //Name of the day
            var dayName = date.DayOfWeek.ToString();
            Console.WriteLine($"[DEBUG] Day of week: {dayName}");

            //Search the schedule
            var schedule = facility.FacilitySchedules
                .FirstOrDefault(s => s.Day_of_Week.Equals(dayName, StringComparison.OrdinalIgnoreCase));

            if (schedule == null)
            {
                Console.WriteLine($"[DEBUG] No schedule found for {dayName}");
                Console.WriteLine($"[DEBUG] Available schedules:");
                foreach (var s in facility.FacilitySchedules)
                {
                    Console.WriteLine($"  - {s.Day_of_Week}: {s.OpeningTime} - {s.ClosingTime}");
                }
                return new List<AvailableSlotsDTO>();
            }

            Console.WriteLine($"[DEBUG] Schedule found - Opening: {schedule.OpeningTime}, Closing: {schedule.ClosingTime}");

            //Get existing reservations
            var reservations = await _reservationRepository.GetReservationsByFacilityAndDateAsync(facilityId, date);
            Console.WriteLine($"[DEBUG] Found {reservations.Count} existing reservations for this date");

            foreach (var r in reservations)
            {
                Console.WriteLine($"  - Reservation {r.ReservationId}: {r.StartTime} - {r.EndTime}");
            }

            var availableSlots = new List<AvailableSlotsDTO>();
            int startHour = schedule.OpeningTime.Hour;
            int endHour = schedule.ClosingTime.Hour;

            Console.WriteLine($"[DEBUG] Generating slots from hour {startHour} to {endHour}");

            //Generate slots hour by hour
            for (int h = startHour; h < endHour; h++)
            {
                var slotStart = new TimeOnly(h, 0);
                var slotEnd = slotStart.AddHours(1);

                //Check if this slot is occupied by ANY reservation
                bool isTaken = reservations.Any(r =>
                {
                    return (r.StartTime.Hour <= h && r.EndTime.Hour > h);
                });

                Console.WriteLine($"[DEBUG] Slot {slotStart}-{slotEnd}: {(isTaken ? "OCCUPIED" : "AVAILABLE")}");

                if (!isTaken)
                {
                    availableSlots.Add(new AvailableSlotsDTO
                    {
                        StartTime = slotStart.ToString("HH:mm"),
                        EndTime = slotEnd.ToString("HH:mm")
                    });
                }
            }

            Console.WriteLine($"[DEBUG] Total available slots: {availableSlots.Count}");
            return availableSlots;
        }

    }
}
