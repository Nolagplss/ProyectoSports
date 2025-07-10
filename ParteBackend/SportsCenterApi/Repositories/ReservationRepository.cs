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


       
    }
}
