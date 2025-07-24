using SportsCenterApi.Models;
using SportsCenterApi.Repositories;
using System.Security.Claims;

namespace SportsCenterApi.Services
{
    public class ReservationService : GenericService<Reservation>, IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository) : base(reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, string? facilityType, string? facilityName, DateOnly? startDate, DateOnly? endDate)
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
    }
}
