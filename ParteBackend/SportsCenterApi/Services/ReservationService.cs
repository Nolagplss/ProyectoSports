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

        public async Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, DateOnly? startDate, DateOnly? endDate)
        {
            return await _reservationRepository.FilterReservationsAsync(userId, startDate, endDate);
        }
    }
}
