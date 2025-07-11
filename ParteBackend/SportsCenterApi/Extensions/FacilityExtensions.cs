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
                Name = facility.Name,
                Type = facility.Type,
                MaxReservationHours = facility.MaxReservationHours,
                MinReservationHours = facility.MinReservationHours,
                CancellationHours = facility.CancellationHours,
                FacilitySchedules = facility.FacilitySchedules.Select(fs => new FacilityScheduleDTO
                {
                    ScheduleId = fs.ScheduleId,
                    FacilityId = fs.FacilityId,
                    Day_of_Week = fs.Day_of_Week,
                    OpeningTime = fs.OpeningTime,
                    ClosingTime = fs.ClosingTime
                }).ToList()
            };
        }

        public static Facility ToFacilityEntity(this FacilityDTO dto)
        {
            return new Facility
            {
                FacilityId = dto.FacilityId == 0 ? 0 : dto.FacilityId,
                Name = dto.Name,
                Type = dto.Type,
                MaxReservationHours = dto.MaxReservationHours,
                MinReservationHours = dto.MinReservationHours,
                CancellationHours = dto.CancellationHours,
                FacilitySchedules = dto.FacilitySchedules
                    .Where(s => s.ScheduleId == 0) // Only news
                    .Select(s => new FacilitySchedule
                    {
                        //Don't include ScheduleId EF generate it and don't include FacilityId EF assign it automatically
                        Day_of_Week = s.Day_of_Week,
                        OpeningTime = s.OpeningTime,
                        ClosingTime = s.ClosingTime
                    }).ToList()
            };
        }
    }
}
