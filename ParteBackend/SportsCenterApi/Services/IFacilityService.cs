using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Services
{
    public interface IFacilityService : IGenericService<Facility>
    {
        Task<IEnumerable<FacilityDTO>> GetAvailableFacilitiesAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime);
        Task<IEnumerable<FacilityDTO>> GetFacilitiesByNameAsync(string name);
    }
}
