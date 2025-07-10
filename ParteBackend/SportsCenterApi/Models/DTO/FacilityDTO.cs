namespace SportsCenterApi.Models.DTO
{
    public class FacilityDTO
    {
        public int FacilityId { get; set; }
        public string Type { get; set; } = null!;
        public int MaxReservationHours { get; set; }
        public int MinReservationHours { get; set; }
        public int CancellationHours { get; set; }

        public List<FacilityScheduleDTO> FacilitySchedules { get; set; } = new();
    }
}
