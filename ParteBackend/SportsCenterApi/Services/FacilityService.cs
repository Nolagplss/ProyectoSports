using SportsCenterApi.Extensions;
using SportsCenterApi.Models;
using SportsCenterApi.Models.DTO;
using SportsCenterApi.Repositories;

namespace SportsCenterApi.Services
{
    public class FacilityService : GenericService<Facility>, IFacilityService
    {
        private readonly IFacilitiesRepository _repository;

        public FacilityService(IFacilitiesRepository repository) : base(repository)
        {
            _repository = repository;
        }


        public async Task<IEnumerable<FacilityDTO>> FilterFacilitiesAsync(string? type, DateOnly? date, TimeOnly? startTime, TimeOnly? endTime)
        {
            var facilities = await _repository.FilterFacilitiesAsync(type, date, startTime, endTime);
            return facilities.Select(f => f.ToFacilityDto()).ToList();
        }

    }
    
    
}
