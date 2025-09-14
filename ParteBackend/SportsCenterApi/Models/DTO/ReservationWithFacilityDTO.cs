namespace SportsCenterApi.Models.DTO
{
    public class ReservationWithFacilityDTO
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public string ReservationDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool PaymentCompleted { get; set; }
        public bool NoShow { get; set; }
    }
}
