using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public class ReservationRepository : GenericRespository<Reservation>, IReservationRepository
    {

        public ReservationRepository(SportsCenterContext context) : base(context)
        {
           
        }

        public async Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.Reservations.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(r => r.UserId == userId);
            }

            if(startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.ReservationDate >= startDate && r.ReservationDate <= endDate);
            }

            return await query.ToListAsync();

        }

        public async Task<bool> HasActiveReservationAsync(int userId, int facilityId, DateOnly fromDate)
        {
            return await _context.Reservations.AnyAsync(r =>
                r.UserId == userId &&
                r.FacilityId == facilityId &&
                r.ReservationDate >= fromDate);
        }

        public async Task<Facility?> GetFacilityByIdAsync(int facilityId)
        {
            return await _context.Facilities.FindAsync(facilityId);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Member)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }



    }
}
