using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetByDateReservationAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);



    }
}
