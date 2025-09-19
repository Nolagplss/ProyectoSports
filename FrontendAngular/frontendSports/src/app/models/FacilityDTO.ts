import { FacilityScheduleDTO } from "./FacilityScheduleDTO";

export interface FacilityDTO {
  facilityId: number;
  name: string;
  type: 'Soccer' | 'Tennis' | 'Padel' | 'Basketball' | 'Pool' | 'Gym';
  maxReservationHours: number;
  minReservationHours: number;
  cancellationHours: number;
  facilitySchedules: FacilityScheduleDTO[];
}
