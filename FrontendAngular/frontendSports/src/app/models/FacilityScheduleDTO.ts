export interface FacilityScheduleDTO {
  scheduleId: number;
  facilityId: number;
  dayOfWeek: string;       // "Monday", "Tuesday", etc.
  openingTime: string;     // "08:00"
  closingTime: string;     // "22:00"
}
