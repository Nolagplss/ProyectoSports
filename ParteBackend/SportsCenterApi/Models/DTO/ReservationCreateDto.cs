using System.ComponentModel.DataAnnotations;

namespace SportsCenterApi.Models.DTO
{
    public class ReservationCreateDto
    {

        public int ReservationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FacilityId { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Invalid date format. Use yyyy-MM-dd.")]
        public string ReservationDate { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Invalid time format. Use HH:mm.")]
        public string StartTime { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}:\d{2}$", ErrorMessage = "Invalid time format. Use HH:mm.")]
        public string EndTime { get; set; }

        [Required]
        public bool PaymentCompleted { get; set; }
    }
}
