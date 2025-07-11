using System.ComponentModel.DataAnnotations;

namespace SportsCenterApi.Models.DTO
{
    public class FacilityDTO
    {
        public int FacilityId { get; set; }
        public string Name { get; set; } = null!;

        [Required]
        [RegularExpression("^(Soccer|Tennis|Padel|Basketball|Pool|Gym)$",
        ErrorMessage = "Type must be one of: Soccer, Tennis, Padel, Basketball, Pool, Gym")]
        public string Type { get; set; } = null!;
        public int MaxReservationHours { get; set; }
        public int MinReservationHours { get; set; }
        public int CancellationHours { get; set; }

        public List<FacilityScheduleDTO> FacilitySchedules { get; set; } = new();
    }
}
