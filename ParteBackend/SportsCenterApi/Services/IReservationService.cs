using SportsCenterApi.Models;
using System.Security.Claims;

namespace SportsCenterApi.Services
{
    public interface IReservationService : IGenericService<Reservation>
    {

        Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, DateOnly? startDate, DateOnly? endDate);

        Task<Reservation> CreateReservationWithValidationAsync(Reservation reservation, bool isAdmin);

        Task PenalizeMemberIfLateCancellationAsync(Reservation reservation);


    }
}
