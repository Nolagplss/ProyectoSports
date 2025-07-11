using SportsCenterApi.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace SportsCenterApi.Models.DTO
{
    public class FacilityScheduleDTO
    {
        public int ScheduleId { get; set; }

        [Required]
        public int FacilityId { get; set; }

        [Required]
        public string Day_of_Week { get; set; } = null!;

        [Required]
        public TimeOnly OpeningTime { get; set; }

        [Required]
        public TimeOnly ClosingTime { get; set; }

    }
}
