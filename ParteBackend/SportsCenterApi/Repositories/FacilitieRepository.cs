using Microsoft.EntityFrameworkCore;
using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public class FacilitiesRepository : GenericRespository<Facility>, IFacilitiesRepository
    {
        private SportsCenterContext _context;
        public FacilitiesRepository(SportsCenterContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Facility>> FilterFacilitiesAsync(string? type, DateOnly? date, TimeOnly? startTime, TimeOnly? endTime)
        {
            var query = _context.Facilities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(f => f.Name.Contains(type));
            }

            if (date.HasValue && startTime.HasValue && endTime.HasValue)
            {
                var reservedIds = await _context.Reservations
                    .Where(r => r.ReservationDate == date &&
                        ((startTime >= r.StartTime && startTime < r.EndTime) ||
                         (endTime > r.StartTime && endTime <= r.EndTime)))
                    .Select(r => r.FacilityId)
                    .ToListAsync();

                query = query.Where(f => !reservedIds.Contains(f.FacilityId));
            }

            return await query.ToListAsync();
        }

    }
}
