using SportsCenterApi.Models;

namespace SportsCenterApi.Services
{
    public interface IReservationService : IGenericService<Reservation>
    {

        Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, DateOnly? startDate, DateOnly? endDate);

    }
}
