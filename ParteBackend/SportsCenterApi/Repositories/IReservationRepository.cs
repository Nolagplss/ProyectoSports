using SportsCenterApi.Models;
using System.Security.Claims;

namespace SportsCenterApi.Repositories
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, DateOnly? startDate, DateOnly? endDate);


        Task<bool> HasActiveReservationAsync(int userId, int facilityId, DateOnly fromDate);
        Task<Facility?> GetFacilityByIdAsync(int facilityId);
        Task<User?> GetUserByIdAsync(int userId);

    }
}
