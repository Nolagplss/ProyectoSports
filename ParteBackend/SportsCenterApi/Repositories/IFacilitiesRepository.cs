using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public interface IFacilitiesRepository : IGenericRepository<Facility>
    {
        Task<IEnumerable<Facility>> GetAvailableFacilitiesAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime);

        Task<IEnumerable<Facility>> GetFacilitiesByNameAsync(string name);


    }
}
