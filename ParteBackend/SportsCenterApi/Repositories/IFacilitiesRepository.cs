using SportsCenterApi.Models;

namespace SportsCenterApi.Repositories
{
    public interface IFacilitiesRepository : IGenericRepository<Facility>
    {

        Task<IEnumerable<Facility>> FilterFacilitiesAsync(string? name, DateOnly? date, TimeOnly? startTime, TimeOnly? endTime);

    }
}
