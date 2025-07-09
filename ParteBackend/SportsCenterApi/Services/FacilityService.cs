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

      
        public async Task<IEnumerable<FacilityDTO>> GetAvailableFacilitiesAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var facilities = await _repository.GetAvailableFacilitiesAsync(date, startTime, endTime);

            return facilities.Select(f => new FacilityDTO
            {
                FacilityId = f.FacilityId,
                Name = f.Name,
                MaxReservationHours = f.MaxReservationHours,
                MinReservationHours = f.MinReservationHours,
                CancellationHours = f.CancellationHours,
                FacilitySchedules = f.FacilitySchedules.Select(fs => new FacilityScheduleDTO
                {
                    ScheduleId = fs.ScheduleId,
                    OpeningTime = fs.OpeningTime,
                    ClosingTime = fs.ClosingTime
                }).ToList()
            });

        }

        public async Task<IEnumerable<FacilityDTO>> GetFacilitiesByNameAsync(string name)
        {
            var facilities = await _repository.GetFacilitiesByNameAsync(name);

            return facilities.Select(f => new FacilityDTO
            {
                FacilityId = f.FacilityId,
                Name = f.Name,
                MaxReservationHours = f.MaxReservationHours,
                MinReservationHours = f.MinReservationHours,
                CancellationHours = f.CancellationHours,
                FacilitySchedules = f.FacilitySchedules.Select(fs => new FacilityScheduleDTO
                {
                    ScheduleId = fs.ScheduleId,
                    OpeningTime = fs.OpeningTime,
                    ClosingTime = fs.ClosingTime
                }).ToList()
            });
        }

    }
    
    
}
