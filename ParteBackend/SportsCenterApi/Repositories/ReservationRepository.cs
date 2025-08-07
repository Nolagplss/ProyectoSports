using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public class ReservationRepository : GenericRespository<Reservation>, IReservationRepository
    {

        public ReservationRepository(SportsCenterContext context) : base(context)
        {
           
        }

        public async Task<IEnumerable<Reservation>> FilterReservationsAsync(int? userId, string? facilityType, string? facilityName, DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.Reservations.AsQueryable();

            //Filter by id
            if (userId.HasValue)
            {
                query = query.Where(r => r.UserId == userId);
            }

            //Filter by date
            if(startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.ReservationDate >= startDate && r.ReservationDate <= endDate);
            }

            //Filter by type
            if(!string.IsNullOrEmpty(facilityType))
            {
                query = query.Include(r => r.Facility)
                    .Where(r => r.Facility.Type == facilityType);
            }

            //Filter by Name
            if (!string.IsNullOrEmpty(facilityName))
            {
                query = query.Include(r => r.Facility)
                    .Where(r => r.Facility.Name == facilityName);
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

        //Update the reservation
        public async Task<Reservation?> UpdateAsyncReservation(Reservation reservation)
        {
            _context.Entry(reservation).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return reservation;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(reservation.ReservationId))
                    return null;
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reservations.AnyAsync(r => r.ReservationId == id);
        }

    }
}
