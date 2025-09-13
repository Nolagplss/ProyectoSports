export interface ReservationResponse{
  reservationId: number;
  userId: number;
  facilityId: number;
  facilityName: string;
  facilityType: string;
  reservationDate: string;
  startTime: string;
  endTime: string;
  paymentCompleted: boolean;
  noShow: boolean;
  createdAt: string;
  updatedAt: string;
}