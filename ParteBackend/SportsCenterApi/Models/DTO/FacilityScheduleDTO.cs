using SportsCenterApi.Models.DTO;

namespace SportsCenterApi.Models.DTO
{
    public class FacilityScheduleDTO
    {
        public int ScheduleId { get; set; }
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
    }
}
