using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Services
{
    public interface IFacilityService : IGenericService<Facility>
    {
        Task<IEnumerable<FacilityDTO>> FilterFacilitiesAsync(string? type, DateOnly? date, TimeOnly? startTime, TimeOnly? endTime);
    }
}
