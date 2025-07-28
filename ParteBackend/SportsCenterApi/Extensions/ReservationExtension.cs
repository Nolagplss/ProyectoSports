using SportsCenterApi.Models.DTO;
using SportsCenterApi.Models;

namespace SportsCenterApi.Extensions
{
    public static class ReservationExtension
    {

        public static ReservationResponseDTO ToReservationResponseDTO(this Reservation reservation)
        {
            return new ReservationResponseDTO()
            {
                UserId = reservation.UserId,
                FacilityId = reservation.FacilityId,
                ReservationDate = reservation.ReservationDate.ToString("yyyy-MM-dd"),
                StartTime = reservation.StartTime.ToString("HH:mm"),
                EndTime = reservation.EndTime.ToString("HH:mm"),
                PaymentCompleted = reservation.PaymentCompleted ?? false

            };
        }

    }
}
