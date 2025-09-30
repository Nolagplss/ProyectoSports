using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using System.Security.Claims;

namespace SportsCenterApi.Repositories
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<IEnumerable<ReservationWithFacilityDTO>> FilterReservationsAsync(int? userId, string? facilityType, string? facilityName, DateOnly? startDate, DateOnly? endDate);


        Task<bool> HasActiveReservationAsync(int userId, int facilityId, DateOnly fromDate);
        Task<Facility?> GetFacilityByIdAsync(int facilityId);
        Task<User?> GetUserByIdAsync(int userId);

        Task<Reservation?> UpdateAsyncReservation(Reservation reservation);

        Task<IEnumerable<Reservation>> GetAllWithFacilitiesAsync();

        Task<List<Reservation>> GetReservationsByFacilityAndDateAsync(int facilityId, DateOnly date);


    }
}
