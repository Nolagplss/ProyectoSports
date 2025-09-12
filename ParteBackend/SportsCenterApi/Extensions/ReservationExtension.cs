using SportsCenterApi.Models.DTO;
using SportsCenterApi.Models;

namespace SportsCenterApi.Extensions
{
    public static class ReservationExtension
    {

        public static Reservation ToReservationEntity(this ReservationCreateDto dto)
        {
            return new Reservation()
            {
                ReservationId = dto.ReservationId,
                UserId = dto.UserId,
                FacilityId = dto.FacilityId,
                ReservationDate = DateOnly.FromDateTime(DateTime.ParseExact(dto.ReservationDate, "yyyy-MM-dd", null)),
                StartTime = TimeOnly.Parse(dto.StartTime),
                EndTime = TimeOnly.Parse(dto.EndTime),
                PaymentCompleted = dto.PaymentCompleted

            };

        }

        public static ReservationResponseDTO ToReservationResponseDTO(this Reservation reservation)
        {
            return new ReservationResponseDTO()
            {
                ReservationId = reservation.ReservationId,
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
