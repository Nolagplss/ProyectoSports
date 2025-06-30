using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public class ReservationRepository : GenericRespository<Reservation>, IReservationRepository
    {
        private readonly SportsCenterContext _context;

        public ReservationRepository(SportsCenterContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetByDateReservationAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.Reservations
                .Where(r => r.ReservationDate >= startDate && r.ReservationDate <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

    }
}
