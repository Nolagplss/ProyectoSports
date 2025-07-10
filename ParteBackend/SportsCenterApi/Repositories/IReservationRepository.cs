using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, DateOnly? startDate, DateOnly? endDate);



    }
}
