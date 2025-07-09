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


        //Get available facilities
        public async Task<IEnumerable<Facility>> GetAvailableFacilitiesAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            //First take the reservations available
            var reservedFacilityIds = await _context.Reservations
              .Where(r => r.ReservationDate == date &&
                      ((startTime >= r.StartTime && startTime < r.EndTime) ||
                       (endTime > r.StartTime && endTime <= r.EndTime)))
             .Select(r => r.FacilityId)
             .ToListAsync();

            //Second take the facilities
            return await _context.Facilities
             .Where(f => !reservedFacilityIds.Contains(f.FacilityId))
             .ToListAsync();
        }

        public async Task<IEnumerable<Facility>> GetFacilitiesByNameAsync(string name)
        {
            return await _context.Facilities
                .Where(f => f.Name == name)
                .ToListAsync();
        }

    }
}
