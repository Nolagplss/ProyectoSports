using SportsCenterApi.Models.DTO;
using SportsCenterApi.Models;

namespace SportsCenterApi.Extensions
{
    public static class FacilityExtensions
    {
        public static FacilityDTO ToFacilityDto(this Facility facility)
        {
            return new FacilityDTO
            {
                FacilityId = facility.FacilityId,
                Type = facility.Type,
                MaxReservationHours = facility.MaxReservationHours,
                MinReservationHours = facility.MinReservationHours,
                CancellationHours = facility.CancellationHours,
                FacilitySchedules = facility.FacilitySchedules.Select(fs => new FacilityScheduleDTO
                {
                    ScheduleId = fs.ScheduleId,
                    OpeningTime = fs.OpeningTime,
                    ClosingTime = fs.ClosingTime
                }).ToList()
            };
        }
    }
}
