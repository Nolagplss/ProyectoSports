using SportsCenterApi.Models;

namespace SportsCenterApi.Services
{
    public interface IReservationService : IGenericService<Reservation>
    {

        Task<IEnumerable<Reservation>> GetByDateReservationAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId);

    }
}
