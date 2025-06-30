using SportsCenterApi.Models;
using SportsCenterApi.Repositories;

namespace SportsCenterApi.Services
{
    public class ReservationService : GenericService<Reservation>, IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository) : base(reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<Reservation>> GetByDateReservationAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _reservationRepository.GetByDateReservationAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            return await _reservationRepository.GetByUserIdAsync(userId);
        }
    }
}
