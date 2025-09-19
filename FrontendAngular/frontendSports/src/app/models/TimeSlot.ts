export interface TimeSlot {
  startTime: string;
  endTime: string;
  duration: number;
  isOccupied?: boolean;  // opcional
  isDisabled?: boolean;  // opcional
}