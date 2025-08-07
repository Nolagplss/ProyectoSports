using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using System.Security.Claims;

namespace SportsCenterApi.Services
{
    public interface IReservationService : IGenericService<Reservation>
    {

        Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, string? facilityType, string? facilityName, DateOnly? startDate, DateOnly? endDate);

        Task<Reservation> CreateReservationWithValidationAsync(Reservation reservation, bool isAdmin);

        Task PenalizeMemberIfLateCancellationAsync(Reservation reservation);

        Task MarkNoShowAsync(int reservationId);

        Task<Reservation?> UpdateAsyncReservation(int id, ReservationCreateDto dto);
    }
}
