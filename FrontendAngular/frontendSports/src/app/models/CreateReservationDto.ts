export interface CreateReservationDto{
  reservationId: number;
  userId: number;
  facilityId: number;
  reservationDate: string;
  startTime: string;
  endTime: string;
  paymentCompleted: boolean;
  noShow: boolean;
}